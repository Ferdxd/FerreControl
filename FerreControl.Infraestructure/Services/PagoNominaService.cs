using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class PagoNominaService
        : IPagoNominaService
    {
        private readonly IPagoNominaRepository
            _pagoRepository;

        private readonly INominaDetalleRepository
            _detalleRepository;


        public PagoNominaService(
            IPagoNominaRepository pagoRepository,
            INominaDetalleRepository detalleRepository)
        {
            _pagoRepository =
                pagoRepository;

            _detalleRepository =
                detalleRepository;
        }


        public async Task<List<PagoNominaDto>>
            ObtenerPorDetalleAsync(
                int nominaDetalleId)
        {
            var pagos =
                await _pagoRepository
                    .ObtenerPorDetalleAsync(
                        nominaDetalleId);


            return pagos
                .Select(MapearDto)
                .ToList();
        }


        public async Task<PagoNominaDto>
            RegistrarAsync(
                int nominaDetalleId,
                CrearPagoNominaDto dto)
        {
            var detalle =
                await _detalleRepository
                    .ObtenerPorIdAsync(
                        nominaDetalleId);

            if (detalle == null)
            {
                throw new InvalidOperationException(
                    "Detalle de nómina no encontrado.");
            }


            if (dto.Monto <= 0)
            {
                throw new InvalidOperationException(
                    "El monto debe ser mayor a cero.");
            }


            if (string.IsNullOrWhiteSpace(
                dto.MetodoPago))
            {
                throw new InvalidOperationException(
                    "Debe indicar el método de pago.");
            }


            long montoCentavos =
                ConvertirCentavos(dto.Monto);


            long pagado =
                await _pagoRepository
                    .ObtenerTotalPagadoCentavosAsync(
                        nominaDetalleId);


            long pendiente =
                detalle.TotalPagarCentavos -
                pagado;


            if (pendiente <= 0)
            {
                throw new InvalidOperationException(
                    "Esta nómina ya se encuentra pagada.");
            }


            if (montoCentavos > pendiente)
            {
                throw new InvalidOperationException(
                    "El pago no puede ser mayor al saldo pendiente.");
            }


            var pago = new PagoNomina
            {
                NominaDetalleID =
                    nominaDetalleId,

                MontoCentavos =
                    montoCentavos,

                FechaPago =
                    dto.FechaPago,

                MetodoPago =
                    dto.MetodoPago.Trim(),

                Referencia =
                    dto.Referencia?.Trim(),

                Observacion =
                    dto.Observacion?.Trim(),

                Estado = true,

                FechaCreacion =
                    DateTime.Now
            };


            await _pagoRepository
                .AgregarAsync(pago);


            return MapearDto(pago);
        }


        public async Task<PagoNominaDto>
            AnularAsync(int id)
        {
            var pago =
                await _pagoRepository
                    .ObtenerPorIdAsync(id);

            if (pago == null)
            {
                throw new InvalidOperationException(
                    "Pago no encontrado.");
            }


            if (!pago.Estado)
            {
                throw new InvalidOperationException(
                    "El pago ya se encuentra anulado.");
            }


            pago.Estado = false;


            await _pagoRepository
                .ActualizarAsync(pago);


            return MapearDto(pago);
        }


        private static PagoNominaDto MapearDto(
            PagoNomina pago)
        {
            return new PagoNominaDto
            {
                ID = pago.ID,

                NominaDetalleID =
                    pago.NominaDetalleID,

                Monto =
                    pago.MontoCentavos / 100m,

                FechaPago =
                    pago.FechaPago,

                MetodoPago =
                    pago.MetodoPago,

                Referencia =
                    pago.Referencia,

                Observacion =
                    pago.Observacion,

                Estado =
                    pago.Estado,

                FechaCreacion =
                    pago.FechaCreacion
            };
        }


        private static long ConvertirCentavos(
            decimal monto)
        {
            return (long)Math.Round(
                monto * 100,
                MidpointRounding.AwayFromZero);
        }
    }
}