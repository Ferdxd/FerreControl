using FerreControl.Application.DTOs.Adelantos;

namespace FerreControl.Application.Interfaces.IService;

public interface IAdelantoEmpleadoService
{
    Task<List<AdelantoEmpleadoDto>>
        ObtenerPorEmpleadoAsync(
            int empleadoId);

    Task<List<AdelantoEmpleadoDto>>
        ObtenerPendientesPorEmpleadoAsync(
            int empleadoId);

    Task<AdelantoEmpleadoDto?>
        ObtenerPorIdAsync(
            int id);

    Task<AdelantoEmpleadoDto>
        CrearAsync(
            int empleadoId,
            CrearAdelantoEmpleadoDto dto);

    Task<AdelantoEmpleadoDto>
        AnularAsync(
            int id,
            AnularAdelantoEmpleadoDto dto);
}