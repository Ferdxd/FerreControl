using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository;

public interface IReglaHoraExtraRepository
{
    Task<List<ReglaHoraExtra>> ObtenerPorNegocioAsync(int negocioId);
    Task<ReglaHoraExtra?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(ReglaHoraExtra regla);
}
