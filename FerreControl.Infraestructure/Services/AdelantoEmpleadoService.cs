using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Constants;
using FerreControl.Domain.Entities;

namespace FerreControl.Infrastructure.Services;

public class AdelantoEmpleadoService
    : IAdelantoEmpleadoService
{
    private readonly IAdelantoEmpleadoRepository
        _adelantoRepository;

    private readonly IAdelantoAplicacionRepository
        _aplicacionRepository;

    private readonly IEmpleadoRepository
        _empleadoRepository;


    public AdelantoEmpleadoService(
        IAdelantoEmpleadoRepository adelantoRepository,
        IAdelantoAplicacionRepository aplicacionRepository,
        IEmpleadoRepository empleadoRepository)
    {
        _adelantoRepository =
            adelantoRepository;

        _aplicacionRepository =
            aplicacionRepository;

        _empleadoRepository =
            empleadoRepository;
    }


    public async Task<List<AdelantoEmpleadoDto>>
        ObtenerPorEmpleadoAsync(
            int empleadoId)
    {
        var adelantos =
            await _adelantoRepository
                .ObtenerPorEmpleadoAsync(
                    empleadoId);


        var totales =
            await _aplicacionRepository
                .ObtenerTotalesPorAdelantosAsync(
                    adelantos.Select(x => x.ID));


        return adelantos
            .Select(x =>
                MapearDto(
                    x,
                    totales.GetValueOrDefault(
                        x.ID)))
            .ToList();
    }


    public async Task<List<AdelantoEmpleadoDto>>
        ObtenerPendientesPorEmpleadoAsync(
            int empleadoId)
    {
        var adelantos =
            await _adelantoRepository
                .ObtenerPendientesPorEmpleadoAsync(
                    empleadoId);


        var totales =
            await _aplicacionRepository
                .ObtenerTotalesPorAdelantosAsync(
                    adelantos.Select(x => x.ID));


        return adelantos
            .Select(x =>
                MapearDto(
                    x,
                    totales.GetValueOrDefault(
                        x.ID)))
            .ToList();
    }


    public async Task<AdelantoEmpleadoDto?>
        ObtenerPorIdAsync(
            int id)
    {
        var adelanto =
            await _adelantoRepository
                .ObtenerPorIdAsync(id);


        if (adelanto == null)
            return null;


        var aplicado =
            await _aplicacionRepository
                .ObtenerTotalAplicadoCentavosAsync(
                    adelanto.ID);


        return MapearDto(
            adelanto,
            aplicado);
    }


    public async Task<AdelantoEmpleadoDto>
        CrearAsync(
            int empleadoId,
            CrearAdelantoEmpleadoDto dto)
    {
        var empleado =
            await _empleadoRepository
                .ObtenerPorIdAsync(
                    empleadoId);


        if (empleado == null)
            throw new InvalidOperationException(
                "Empleado no encontrado.");


        if (!empleado.Estado)
            throw new InvalidOperationException(
                "No se puede registrar un adelanto a un empleado inactivo.");


        if (dto.Monto <= 0)
            throw new InvalidOperationException(
                "El monto del adelanto debe ser mayor a cero.");


        if (dto.FechaAdelanto == default)
            throw new InvalidOperationException(
                "Debe indicar la fecha del adelanto.");


        if (dto.FechaAdelanto.Date >
            DateTime.Today)
        {
            throw new InvalidOperationException(
                "La fecha del adelanto no puede ser futura.");
        }


        if (string.IsNullOrWhiteSpace(
            dto.MetodoEntrega))
        {
            throw new InvalidOperationException(
                "Debe indicar el método de entrega.");
        }


        var adelanto =
            new AdelantoEmpleado
            {
                EmpleadoID =
                    empleadoId,

                MontoCentavos =
                    A_Centavos(
                        dto.Monto),

                FechaAdelanto =
                    dto.FechaAdelanto.Date,

                MetodoEntrega =
                    dto.MetodoEntrega
                        .Trim()
                        .ToUpper(),

                Referencia =
                    dto.Referencia?.Trim(),

                Observacion =
                    dto.Observacion?.Trim(),

                Estado =
                    EstadosAdelanto.PENDIENTE,

                FechaCreacion =
                    DateTime.Now
            };


        await _adelantoRepository
            .AgregarAsync(
                adelanto);


        return MapearDto(
            adelanto,
            0);
    }


    public async Task<AdelantoEmpleadoDto>
        AnularAsync(
            int id,
            AnularAdelantoEmpleadoDto dto)
    {
        var adelanto =
            await _adelantoRepository
                .ObtenerPorIdAsync(
                    id);


        if (adelanto == null)
            throw new InvalidOperationException(
                "Adelanto no encontrado.");


        if (adelanto.Estado ==
            EstadosAdelanto.ANULADO)
        {
            throw new InvalidOperationException(
                "El adelanto ya se encuentra anulado.");
        }


        var aplicado =
            await _aplicacionRepository
                .ObtenerTotalAplicadoCentavosAsync(
                    adelanto.ID);


        if (aplicado > 0)
        {
            throw new InvalidOperationException(
                "No puede anular el adelanto porque tiene aplicaciones activas. Anule primero sus aplicaciones.");
        }


        if (string.IsNullOrWhiteSpace(
            dto.Motivo))
        {
            throw new InvalidOperationException(
                "Debe indicar el motivo de anulación.");
        }


        adelanto.Estado =
            EstadosAdelanto.ANULADO;

        adelanto.FechaAnulacion =
            DateTime.Now;

        adelanto.MotivoAnulacion =
            dto.Motivo.Trim();


        await _adelantoRepository
            .ActualizarAsync(
                adelanto);


        return MapearDto(
            adelanto,
            0);
    }


    private AdelantoEmpleadoDto MapearDto(
        AdelantoEmpleado adelanto,
        long totalAplicadoCentavos)
    {
        var saldo =
            adelanto.MontoCentavos
            -
            totalAplicadoCentavos;


        if (saldo < 0)
            saldo = 0;


        return new AdelantoEmpleadoDto
        {
            ID =
                adelanto.ID,

            EmpleadoID =
                adelanto.EmpleadoID,

            Monto =
                adelanto.MontoCentavos / 100m,

            TotalAplicado =
                totalAplicadoCentavos / 100m,

            SaldoPendiente =
                saldo / 100m,

            FechaAdelanto =
                adelanto.FechaAdelanto,

            MetodoEntrega =
                adelanto.MetodoEntrega,

            Referencia =
                adelanto.Referencia,

            Observacion =
                adelanto.Observacion,

            Estado =
                adelanto.Estado,

            FechaCreacion =
                adelanto.FechaCreacion,

            FechaAnulacion =
                adelanto.FechaAnulacion,

            MotivoAnulacion =
                adelanto.MotivoAnulacion
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