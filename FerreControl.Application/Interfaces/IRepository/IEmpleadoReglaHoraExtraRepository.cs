using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository;

public interface IEmpleadoReglaHoraExtraRepository
{
    Task<List<EmpleadoReglaHoraExtra>> ObtenerHistorialAsync(int empleadoId);
    Task<EmpleadoReglaHoraExtra?> ObtenerActualAsync(int empleadoId);
    Task<EmpleadoReglaHoraExtra?> ObtenerVigenteEnFechaAsync(
        int empleadoId,
        DateTime fecha);

    Task AgregarAsync(EmpleadoReglaHoraExtra configuracion);
    Task ActualizarAsync(EmpleadoReglaHoraExtra configuracion);
}
