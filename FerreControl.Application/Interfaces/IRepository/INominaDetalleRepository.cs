using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface INominaDetalleRepository
    {
        Task<List<NominaDetalle>> ObtenerPorNominaAsync(int nominaId);

        Task<NominaDetalle?> ObtenerPorIdAsync(int id);

        Task<NominaDetalle?> ObtenerPorNominaEmpleadoAsync(
            int nominaId,
            int empleadoId);

        Task AgregarAsync(NominaDetalle detalle);

        Task ActualizarAsync(NominaDetalle detalle);
    }
}