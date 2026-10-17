using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class NominaDetalleService
        : INominaDetalleService
    {
        private readonly INominaDetalleRepository
            _detalleRepository;

        private readonly INominaRepository
            _nominaRepository;

        private readonly IEmpleadoRepository
            _empleadoRepository;

        private readonly ISalarioEmpleadoRepository
            _salarioRepository;

        private readonly IPagoNominaRepository
            _pagoRepository;


        public NominaDetalleService(
            INominaDetalleRepository detalleRepository,
            INominaRepository nominaRepository,
            IEmpleadoRepository empleadoRepository,
            ISalarioEmpleadoRepository salarioRepository,
            IPagoNominaRepository pagoRepository)
        {
            _detalleRepository =
                detalleRepository;

            _nominaRepository =
                nominaRepository;

            _empleadoRepository =
                empleadoRepository;

            _salarioRepository =
                salarioRepository;

            _pagoRepository =
                pagoRepository;
        }


        public async Task<List<NominaDetalleDto>>
            ObtenerPorNominaAsync(
                int nominaId)
        {
            var detalles =
                await _detalleRepository
                    .ObtenerPorNominaAsync(
                        nominaId);


            var resultado =
                new List<NominaDetalleDto>();


            foreach (var detalle in detalles)
            {
                resultado.Add(
                    await MapearDtoAsync(
                        detalle));
            }


            return resultado;
        }


        public async Task<NominaDetalleDto?>
            ObtenerPorIdAsync(
                int id)
        {
            var detalle =
                await _detalleRepository
                    .ObtenerPorIdAsync(
                        id);


            if (detalle == null)
                return null;


            return await MapearDtoAsync(
                detalle);
        }


        public async Task<NominaDetalleDto>
            CrearAsync(
                int nominaId,
                CrearNominaDetalleDto dto)
        {
            var nomina =
                await _nominaRepository
                    .ObtenerPorIdAsync(
                        nominaId);


            if (nomina == null)
            {
                throw new InvalidOperationException(
                    "Nómina no encontrada.");
            }


            if (nomina.Estado != "BORRADOR")
            {
                throw new InvalidOperationException(
                    "Solo se pueden agregar empleados a una nómina en estado BORRADOR.");
            }


            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        dto.EmpleadoID);


            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "Empleado no encontrado.");
            }


            if (!empleado.Estado)
            {
                throw new InvalidOperationException(
                    "El empleado se encuentra inactivo.");
            }


            if (empleado.NegocioID !=
                nomina.NegocioID)
            {
                throw new InvalidOperationException(
                    "El empleado no pertenece al negocio de la nómina.");
            }


            var existente =
                await _detalleRepository
                    .ObtenerPorNominaEmpleadoAsync(
                        nominaId,
                        dto.EmpleadoID);


            if (existente != null)
            {
                throw new InvalidOperationException(
                    "El empleado ya fue agregado a esta nómina.");
            }


            var salario =
                await _salarioRepository
                    .ObtenerActualAsync(
                        dto.EmpleadoID);


            if (salario == null)
            {
                throw new InvalidOperationException(
                    "El empleado no tiene un salario vigente.");
            }


            long bonificaciones =
                ConvertirCentavos(
                    dto.Bonificaciones);


            long horasExtra =
                ConvertirCentavos(
                    dto.HorasExtra);


            long otrosDescuentos =
                ConvertirCentavos(
                    dto.OtrosDescuentos);


            if (bonificaciones < 0)
            {
                throw new InvalidOperationException(
                    "Las bonificaciones no pueden ser negativas.");
            }


            if (horasExtra < 0)
            {
                throw new InvalidOperationException(
                    "Las horas extra no pueden ser negativas.");
            }


            if (otrosDescuentos < 0)
            {
                throw new InvalidOperationException(
                    "Los otros descuentos no pueden ser negativos.");
            }


            long total =
                salario.MontoCentavos
                +
                bonificaciones
                +
                horasExtra
                -
                otrosDescuentos;


            if (total < 0)
            {
                throw new InvalidOperationException(
                    "El total de la nómina no puede ser negativo.");
            }


            var detalle =
                new NominaDetalle
                {
                    NominaID =
                        nominaId,

                    EmpleadoID =
                        empleado.ID,

                    SalarioEmpleadoID =
                        salario.ID,

                    SalarioBaseCentavos =
                        salario.MontoCentavos,

                    BonificacionesCentavos =
                        bonificaciones,

                    HorasExtraCentavos =
                        horasExtra,

                    AdelantosCentavos =
                        0,

                    OtrosDescuentosCentavos =
                        otrosDescuentos,

                    TotalPagarCentavos =
                        total,

                    FechaCreacion =
                        DateTime.Now
                };


            await _detalleRepository
                .AgregarAsync(
                    detalle);


            return await MapearDtoAsync(
                detalle);
        }


        private async Task<NominaDetalleDto>
            MapearDtoAsync(
                NominaDetalle detalle)
        {
            var totalPagado =
                await _pagoRepository
                    .ObtenerTotalPagadoCentavosAsync(
                        detalle.ID);


            var pendiente =
                detalle.TotalPagarCentavos
                -
                totalPagado;


            if (pendiente < 0)
                pendiente = 0;


            return new NominaDetalleDto
            {
                ID =
                    detalle.ID,

                NominaID =
                    detalle.NominaID,

                EmpleadoID =
                    detalle.EmpleadoID,

                SalarioEmpleadoID =
                    detalle.SalarioEmpleadoID,

                SalarioBase =
                    ConvertirDecimal(
                        detalle.SalarioBaseCentavos),

                Bonificaciones =
                    ConvertirDecimal(
                        detalle.BonificacionesCentavos),

                HorasExtra =
                    ConvertirDecimal(
                        detalle.HorasExtraCentavos),

                Adelantos =
                    ConvertirDecimal(
                        detalle.AdelantosCentavos),

                OtrosDescuentos =
                    ConvertirDecimal(
                        detalle.OtrosDescuentosCentavos),

                TotalPagar =
                    ConvertirDecimal(
                        detalle.TotalPagarCentavos),

                TotalPagado =
                    ConvertirDecimal(
                        totalPagado),

                Pendiente =
                    ConvertirDecimal(
                        pendiente),

                FechaCreacion =
                    detalle.FechaCreacion
            };
        }


        private static long ConvertirCentavos(
            decimal monto)
        {
            return (long)Math.Round(
                monto * 100m,
                MidpointRounding.AwayFromZero);
        }


        private static decimal ConvertirDecimal(
            long centavos)
        {
            return centavos / 100m;
        }
    }
}