using FerreControl.Application.DTOs.Nomina;

namespace FerreControl.Application.Interfaces.IService
{
    public interface INominaDetalleService
    {
        Task<List<NominaDetalleDto>> ObtenerPorNominaAsync(
            int nominaId);

        Task<NominaDetalleDto?> ObtenerPorIdAsync(int id);

        Task<NominaDetalleDto> CrearAsync(
            int nominaId,
            CrearNominaDetalleDto dto);
    }
}