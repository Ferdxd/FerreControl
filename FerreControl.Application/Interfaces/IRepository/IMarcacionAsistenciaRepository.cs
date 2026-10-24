using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IMarcacionAsistenciaRepository
    {
        Task<List<MarcacionAsistencia>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha);


        Task<List<MarcacionAsistencia>>
            ObtenerPorFechaAsync(
                DateTime fecha);


        Task<bool> ExisteTipoEnFechaAsync(
            int empleadoId,
            string tipoMarcacion,
            DateTime fecha);


        Task AgregarAsync(
            MarcacionAsistencia marcacion);


        Task AgregarRangoAsync(
            IEnumerable<MarcacionAsistencia> marcaciones);
    }
}