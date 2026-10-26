using FerreControl.Application.DTOs.EmpleadoCredencial;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IEmpleadoCredencialService
    {
        Task<List<EmpleadoCredencialDto>>
            ObtenerPorEmpleadoAsync(
                int empleadoId);


        Task<EmpleadoCredencialDto>
            CrearAsync(
                int empleadoId,
                CrearEmpleadoCredencialDto dto);


        Task<EmpleadoCredencialDto?>
            ActualizarEstadoAsync(
                int credencialId,
                bool estado);


        Task<List<CarnetEmpleadoDto>>
            ObtenerCarnesActivosAsync(
                int negocioId);


        Task<EmpleadoCredencialDto>
            GenerarCodigoBarrasAsync(
                int empleadoId);


        Task<EmpleadoCredencialDto>
            ReemplazarCodigoBarrasAsync(
                int empleadoId);


        Task<EmpleadoCredencialDto?>
            DesactivarCodigoBarrasAsync(
                int empleadoId);


        Task<List<EmpleadoCredencialDto>>
            GenerarCodigosFaltantesAsync(
                int negocioId);
    }
}