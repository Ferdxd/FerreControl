using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Constants;
using FerreControl.Domain.Entities;

namespace FerreControl.Infrastructure.Services;

public class AdelantoAplicacionService
    : IAdelantoAplicacionService
{
    private readonly IAdelantoEmpleadoRepository
        _adelantoRepository;

    private readonly IAdelantoAplicacionRepository
        _aplicacionRepository;

    private readonly INominaDetalleRepository
        _nominaDetalleRepository;

    private readonly INominaRepository
        _nominaRepository;

    private readonly IPagoNominaRepository
        _pagoRepository;


    public AdelantoAplicacionService(
        IAdelantoEmpleadoRepository adelantoRepository,
        IAdelantoAplicacionRepository aplicacionRepository,
        INominaDetalleRepository nominaDetalleRepository,
        INominaRepository nominaRepository,
        IPagoNominaRepository pagoRepository)
    {
        _adelantoRepository =
            adelantoRepository;

        _aplicacionRepository =
            aplicacionRepository;

        _nominaDetalleRepository =
            nominaDetalleRepository;

        _nominaRepository =
            nominaRepository;

        _pagoRepository =
            pagoRepository;
    }


    public async Task<List<AdelantoAplicacionDto>>
        ObtenerPorAdelantoAsync(
            int adelantoId)
    {
        var aplicaciones =
            await _aplicacionRepository
                .ObtenerPorAdelantoAsync(
                    adelantoId);


        return aplicaciones
            .Select(MapearDto)
            .ToList();
    }


    public async Task<List<AdelantoAplicacionDto>>
        ObtenerPorNominaDetalleAsync(
            int nominaDetalleId)
    {
        var aplicaciones =
            await _aplicacionRepository
                .ObtenerPorNominaDetalleAsync(
                    nominaDetalleId);


        return aplicaciones
            .Select(MapearDto)
            .ToList();
    }


    public async Task<AdelantoAplicacionDto>
        AplicarAsync(
            int adelantoId,
            CrearAdelantoAplicacionDto dto)
    {
        var adelanto =
            await _adelantoRepository
                .ObtenerPorIdAsync(
                    adelantoId);


        if (adelanto == null)
            throw new InvalidOperationException(
                "Adelanto no encontrado.");


        if (adelanto.Estado ==
            EstadosAdelanto.ANULADO)
        {
            throw new InvalidOperationException(
                "No se puede aplicar un adelanto anulado.");
        }


        if (adelanto.Estado ==
            EstadosAdelanto.LIQUIDADO)
        {
            throw new InvalidOperationException(
                "El adelanto ya se encuentra liquidado.");
        }


        var detalle =
            await _nominaDetalleRepository
                .ObtenerPorIdAsync(
                    dto.NominaDetalleID);


        if (detalle == null)
            throw new InvalidOperationException(
                "Detalle de nómina no encontrado.");


        if (detalle.EmpleadoID !=
            adelanto.EmpleadoID)
        {
            throw new InvalidOperationException(
                "El adelanto no pertenece al empleado de esta nómina.");
        }


        var nomina =
            await _nominaRepository
                .ObtenerPorIdAsync(
                    detalle.NominaID);


        if (nomina == null)
            throw new InvalidOperationException(
                "Nómina no encontrada.");


        if (nomina.Estado == "CERRADA")
        {
            throw new InvalidOperationException(
                "No se puede modificar una nómina cerrada.");
        }


        var totalPagado =
            await _pagoRepository
                .ObtenerTotalPagadoCentavosAsync(
                    detalle.ID);


        if (totalPagado > 0)
        {
            throw new InvalidOperationException(
                "No se pueden modificar adelantos de una nómina que ya tiene pagos registrados.");
        }


        if (dto.Monto <= 0)
        {
            throw new InvalidOperationException(
                "El monto a aplicar debe ser mayor a cero.");
        }


        var aplicacionExistente =
            await _aplicacionRepository
                .ObtenerActivaAsync(
                    adelanto.ID,
                    detalle.ID);


        if (aplicacionExistente != null)
        {
            throw new InvalidOperationException(
                "Este adelanto ya tiene una aplicación activa en esta nómina.");
        }


        var montoCentavos =
            A_Centavos(
                dto.Monto);


        var totalAplicado =
            await _aplicacionRepository
                .ObtenerTotalAplicadoCentavosAsync(
                    adelanto.ID);


        var saldoAdelanto =
            adelanto.MontoCentavos
            -
            totalAplicado;


        if (montoCentavos >
            saldoAdelanto)
        {
            throw new InvalidOperationException(
                "El monto supera el saldo pendiente del adelanto.");
        }


        var nuevosAdelantos =
            detalle.AdelantosCentavos
            +
            montoCentavos;


        var nuevoTotal =
            detalle.SalarioBaseCentavos
            +
            detalle.BonificacionesCentavos
            +
            detalle.HorasExtraCentavos
            -
            nuevosAdelantos
            -
            detalle.OtrosDescuentosCentavos;


        if (nuevoTotal < 0)
        {
            throw new InvalidOperationException(
                "El adelanto aplicado provoca que el total de la nómina sea negativo.");
        }


        var aplicacion =
            new AdelantoAplicacion
            {
                AdelantoEmpleadoID =
                    adelanto.ID,

                NominaDetalleID =
                    detalle.ID,

                MontoCentavos =
                    montoCentavos,

                FechaAplicacion =
                    DateTime.Now,

                Observacion =
                    dto.Observacion?.Trim(),

                Estado =
                    true,

                FechaCreacion =
                    DateTime.Now
            };


        await _aplicacionRepository
            .AgregarAsync(
                aplicacion);


        detalle.AdelantosCentavos =
            await _aplicacionRepository
                .ObtenerTotalAplicadoNominaCentavosAsync(
                    detalle.ID);


        RecalcularNomina(
            detalle);


        await _nominaDetalleRepository
            .ActualizarAsync(
                detalle);


        await ActualizarEstadoAdelantoAsync(
            adelanto);


        return MapearDto(
            aplicacion);
    }


    public async Task<AdelantoAplicacionDto>
        AnularAsync(
            int id,
            AnularAdelantoAplicacionDto dto)
    {
        var aplicacion =
            await _aplicacionRepository
                .ObtenerPorIdAsync(
                    id);


        if (aplicacion == null)
            throw new InvalidOperationException(
                "Aplicación no encontrada.");


        if (!aplicacion.Estado)
            throw new InvalidOperationException(
                "La aplicación ya se encuentra anulada.");


        if (string.IsNullOrWhiteSpace(
            dto.Motivo))
        {
            throw new InvalidOperationException(
                "Debe indicar el motivo de anulación.");
        }


        var detalle =
            await _nominaDetalleRepository
                .ObtenerPorIdAsync(
                    aplicacion.NominaDetalleID);


        if (detalle == null)
            throw new InvalidOperationException(
                "Detalle de nómina no encontrado.");


        var nomina =
            await _nominaRepository
                .ObtenerPorIdAsync(
                    detalle.NominaID);


        if (nomina == null)
            throw new InvalidOperationException(
                "Nómina no encontrada.");


        if (nomina.Estado == "CERRADA")
        {
            throw new InvalidOperationException(
                "No se puede modificar una nómina cerrada.");
        }


        var totalPagado =
            await _pagoRepository
                .ObtenerTotalPagadoCentavosAsync(
                    detalle.ID);


        if (totalPagado > 0)
        {
            throw new InvalidOperationException(
                "No se puede modificar una nómina que ya tiene pagos.");
        }


        aplicacion.Estado =
            false;

        aplicacion.FechaAnulacion =
            DateTime.Now;

        aplicacion.MotivoAnulacion =
            dto.Motivo.Trim();


        await _aplicacionRepository
            .ActualizarAsync(
                aplicacion);


        detalle.AdelantosCentavos =
            await _aplicacionRepository
                .ObtenerTotalAplicadoNominaCentavosAsync(
                    detalle.ID);


        RecalcularNomina(
            detalle);


        await _nominaDetalleRepository
            .ActualizarAsync(
                detalle);


        var adelanto =
            await _adelantoRepository
                .ObtenerPorIdAsync(
                    aplicacion.AdelantoEmpleadoID);


        if (adelanto != null)
        {
            await ActualizarEstadoAdelantoAsync(
                adelanto);
        }


        return MapearDto(
            aplicacion);
    }


    private async Task ActualizarEstadoAdelantoAsync(
        AdelantoEmpleado adelanto)
    {
        if (adelanto.Estado ==
            EstadosAdelanto.ANULADO)
        {
            return;
        }


        var aplicado =
            await _aplicacionRepository
                .ObtenerTotalAplicadoCentavosAsync(
                    adelanto.ID);


        if (aplicado <= 0)
        {
            adelanto.Estado =
                EstadosAdelanto.PENDIENTE;
        }
        else if (aplicado >=
                 adelanto.MontoCentavos)
        {
            adelanto.Estado =
                EstadosAdelanto.LIQUIDADO;
        }
        else
        {
            adelanto.Estado =
                EstadosAdelanto.PARCIAL;
        }


        await _adelantoRepository
            .ActualizarAsync(
                adelanto);
    }


    private void RecalcularNomina(
        NominaDetalle detalle)
    {
        detalle.TotalPagarCentavos =
            detalle.SalarioBaseCentavos
            +
            detalle.BonificacionesCentavos
            +
            detalle.HorasExtraCentavos
            -
            detalle.AdelantosCentavos
            -
            detalle.OtrosDescuentosCentavos;


        if (detalle.TotalPagarCentavos < 0)
        {
            throw new InvalidOperationException(
                "El total a pagar de la nómina no puede ser negativo.");
        }
    }


    private AdelantoAplicacionDto MapearDto(
        AdelantoAplicacion aplicacion)
    {
        return new AdelantoAplicacionDto
        {
            ID =
                aplicacion.ID,

            AdelantoEmpleadoID =
                aplicacion.AdelantoEmpleadoID,

            NominaDetalleID =
                aplicacion.NominaDetalleID,

            Monto =
                aplicacion.MontoCentavos / 100m,

            FechaAplicacion =
                aplicacion.FechaAplicacion,

            Observacion =
                aplicacion.Observacion,

            Estado =
                aplicacion.Estado,

            FechaCreacion =
                aplicacion.FechaCreacion,

            FechaAnulacion =
                aplicacion.FechaAnulacion,

            MotivoAnulacion =
                aplicacion.MotivoAnulacion
        };
    }


    private long A_Centavos(
        decimal monto)
    {
        return (long)Math.Round(
            monto * 100m,
            MidpointRounding.AwayFromZero);
    }
}