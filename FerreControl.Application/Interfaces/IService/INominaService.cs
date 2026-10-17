using FerreControl.Application.DTOs.Nomina;

namespace FerreControl.Application.Interfaces.IService
{
    public interface INominaService
    {
        Task<List<NominaDto>> ObtenerPorNegocioAsync(
            int negocioId);

        Task<NominaDto?> ObtenerPorIdAsync(int id);

        Task<NominaDto> CrearAsync(
            CrearNominaDto dto);

        Task<NominaDto> ActualizarEstadoAsync(
            int id,
            ActualizarEstadoNominaDto dto);
    }
}