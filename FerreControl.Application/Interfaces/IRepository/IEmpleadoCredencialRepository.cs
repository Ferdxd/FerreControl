using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IEmpleadoCredencialRepository
    {
        Task<List<EmpleadoCredencial>>
            ObtenerPorEmpleadoAsync(
                int empleadoId);


        Task<EmpleadoCredencial?>
            ObtenerPorIdAsync(
                int id);


        Task<EmpleadoCredencial?>
            ObtenerActivaPorCodigoAsync(
                string tipoCredencial,
                string codigo);


        Task<EmpleadoCredencial?>
            ObtenerActivaPorEmpleadoYTipoAsync(
                int empleadoId,
                string tipoCredencial);


        Task<List<EmpleadoCredencial>>
            ObtenerActivasPorNegocioYTipoAsync(
                int negocioId,
                string tipoCredencial);


        Task<bool>
            ExisteCodigoAsync(
                string tipoCredencial,
                string codigo);


        Task AgregarAsync(
            EmpleadoCredencial credencial);


        Task ActualizarAsync(
            EmpleadoCredencial credencial);


        Task ReemplazarAsync(
            EmpleadoCredencial? anterior,
            EmpleadoCredencial nueva);
    }
}