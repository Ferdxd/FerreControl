using FerreControl.Application.DTOs.HorasExtra;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services;

public class HoraExtraApiService
{
    private readonly HttpClient _httpClient;

    public HoraExtraApiService()
    {
        _httpClient =
            new HttpClient
            {
                BaseAddress =
                    new Uri("https://localhost:7217/")
            };
    }

    public async Task<List<ReglaHoraExtraDto>>
        ObtenerReglasAsync(int negocioId)
    {
        var response =
            await _httpClient.GetAsync(
                $"api/horas-extra/reglas/negocio/{negocioId}");

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<ReglaHoraExtraDto>>()
            ?? new List<ReglaHoraExtraDto>();
    }

    public async Task<ReglaHoraExtraDto>
        CrearReglaAsync(CrearReglaHoraExtraDto dto)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/horas-extra/reglas",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<ReglaHoraExtraDto>()
            ?? throw new InvalidOperationException(
                "La API no devolvió la regla creada.");
    }

    public async Task<EmpleadoReglaHoraExtraDto?>
        ObtenerActualAsync(int empleadoId)
    {
        var response =
            await _httpClient.GetAsync(
                $"api/horas-extra/empleados/{empleadoId}/actual");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<EmpleadoReglaHoraExtraDto>();
    }

    public async Task<EmpleadoReglaHoraExtraDto>
        AsignarAsync(
            int empleadoId,
            AsignarReglaHoraExtraDto dto)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                $"api/horas-extra/empleados/{empleadoId}/asignar",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<EmpleadoReglaHoraExtraDto>()
            ?? throw new InvalidOperationException(
                "La API no devolvió la configuración asignada.");
    }

    public async Task<EmpleadoReglaHoraExtraDto>
        DesactivarAsync(
            int empleadoId,
            DesactivarHoraExtraEmpleadoDto dto)
    {
        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/horas-extra/empleados/{empleadoId}/desactivar",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<EmpleadoReglaHoraExtraDto>()
            ?? throw new InvalidOperationException(
                "La API no devolvió la configuración desactivada.");
    }

    public async Task<List<RegistroHoraExtraDto>>
        CalcularAsync(CalcularHorasExtraDto dto)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/horas-extra/calcular",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<RegistroHoraExtraDto>>()
            ?? new List<RegistroHoraExtraDto>();
    }

    public async Task<List<RegistroHoraExtraDto>>
        ObtenerRegistrosAsync(
            int negocioId,
            DateTime desde,
            DateTime hasta)
    {
        string inicio = desde.ToString("yyyy-MM-dd");
        string fin = hasta.ToString("yyyy-MM-dd");

        var response =
            await _httpClient.GetAsync(
                $"api/horas-extra/registros" +
                $"?negocioId={negocioId}" +
                $"&desde={inicio}" +
                $"&hasta={fin}");

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<RegistroHoraExtraDto>>()
            ?? new List<RegistroHoraExtraDto>();
    }

    public async Task<RegistroHoraExtraDto>
        AprobarAsync(
            int registroId,
            AprobarHoraExtraDto dto)
    {
        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/horas-extra/registros/{registroId}/aprobar",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<RegistroHoraExtraDto>()
            ?? throw new InvalidOperationException(
                "La API no devolvió el registro aprobado.");
    }

    public async Task<RegistroHoraExtraDto>
        RechazarAsync(
            int registroId,
            RechazarHoraExtraDto dto)
    {
        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/horas-extra/registros/{registroId}/rechazar",
                dto);

        await ValidarAsync(response);

        return await response.Content
            .ReadFromJsonAsync<RegistroHoraExtraDto>()
            ?? throw new InvalidOperationException(
                "La API no devolvió el registro rechazado.");
    }

    private static async Task ValidarAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var mensaje =
            await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(mensaje))
            mensaje = $"Error HTTP {(int)response.StatusCode}.";

        throw new InvalidOperationException(
            mensaje.Trim('"'));
    }
}
