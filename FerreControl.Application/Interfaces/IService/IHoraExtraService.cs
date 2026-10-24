using FerreControl.Application.DTOs.HorasExtra;

namespace FerreControl.Application.Interfaces.IService;

public interface IHoraExtraService
{
    Task<List<ReglaHoraExtraDto>> ObtenerReglasAsync(int negocioId);

    Task<ReglaHoraExtraDto> CrearReglaAsync(
        CrearReglaHoraExtraDto dto);

    Task<EmpleadoReglaHoraExtraDto?> ObtenerConfiguracionActualAsync(
        int empleadoId);

    Task<EmpleadoReglaHoraExtraDto> AsignarReglaAsync(
        int empleadoId,
        AsignarReglaHoraExtraDto dto);

    Task<EmpleadoReglaHoraExtraDto> DesactivarConfiguracionAsync(
        int empleadoId,
        DesactivarHoraExtraEmpleadoDto dto);

    Task<List<RegistroHoraExtraDto>> CalcularPeriodoAsync(
        CalcularHorasExtraDto dto);

    Task<List<RegistroHoraExtraDto>> ObtenerRegistrosAsync(
        int negocioId,
        DateTime desde,
        DateTime hasta);

    Task<RegistroHoraExtraDto> AprobarAsync(
        int registroId,
        AprobarHoraExtraDto dto);

    Task<RegistroHoraExtraDto> RechazarAsync(
        int registroId,
        RechazarHoraExtraDto dto);

    Task<ResultadoRecalculoNominaHoraExtraDto> RecalcularNominaAsync(
        int nominaId);
}
