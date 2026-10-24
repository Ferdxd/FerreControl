using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository;

public interface IRegistroHoraExtraRepository
{
    Task<RegistroHoraExtra?> ObtenerPorIdAsync(int id);

    Task<RegistroHoraExtra?> ObtenerPorEmpleadoFechaAsync(
        int empleadoId,
        DateTime fecha);

    Task<List<RegistroHoraExtra>> ObtenerPorNegocioPeriodoAsync(
        int negocioId,
        DateTime desde,
        DateTime hasta);

    Task AgregarAsync(RegistroHoraExtra registro);
    Task ActualizarAsync(RegistroHoraExtra registro);
}
