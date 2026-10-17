using FerreControl.Application.DTOs.Nomina;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IPagoNominaService
    {
        Task<List<PagoNominaDto>> ObtenerPorDetalleAsync(
            int nominaDetalleId);

        Task<PagoNominaDto> RegistrarAsync(
            int nominaDetalleId,
            CrearPagoNominaDto dto);

        Task<PagoNominaDto> AnularAsync(int id);
    }
}