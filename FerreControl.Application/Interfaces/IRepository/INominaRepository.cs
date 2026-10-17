using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface INominaRepository
    {
        Task<List<Nomina>> ObtenerPorNegocioAsync(int negocioId);

        Task<Nomina?> ObtenerPorIdAsync(int id);

        Task AgregarAsync(Nomina nomina);

        Task ActualizarAsync(Nomina nomina);
    }
}