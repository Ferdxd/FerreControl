using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository;

public interface IAdelantoEmpleadoRepository
{
    Task<List<AdelantoEmpleado>> ObtenerPorEmpleadoAsync(
        int empleadoId);

    Task<List<AdelantoEmpleado>> ObtenerPendientesPorEmpleadoAsync(
        int empleadoId);

    Task<AdelantoEmpleado?> ObtenerPorIdAsync(
        int id);

    Task AgregarAsync(
        AdelantoEmpleado adelanto);

    Task ActualizarAsync(
        AdelantoEmpleado adelanto);
}