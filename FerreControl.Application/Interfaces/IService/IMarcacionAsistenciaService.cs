using FerreControl.Application.DTOs.MarcacionAsistencia;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IMarcacionAsistenciaService
    {
        Task<MarcacionAsistenciaDto>
            RegistrarAsync(
                RegistrarMarcacionDto dto);


        Task<MarcacionAsistenciaDto>
            RegistrarAutomaticoAsync(
                RegistrarMarcacionAutomaticaDto dto);


        Task<List<MarcacionAsistenciaDto>>
            RegistrarManualAsync(
                RegistrarAsistenciaManualDto dto);


        Task<List<MarcacionAsistenciaDto>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha);


        Task<List<AsistenciaDiariaDto>>
            ObtenerResumenDiarioAsync(
                int negocioId,
                DateTime fecha);
    }
}