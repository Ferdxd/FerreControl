using FerreControl.Application.DTOs.MarcacionAsistencia;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class MarcacionAsistenciaApiService
    {
        private readonly HttpClient
            _httpClient;


        public MarcacionAsistenciaApiService()
        {
            _httpClient =
                new HttpClient
                {
                    BaseAddress =
                        new Uri(
                            "https://localhost:7217/")
                };
        }


        public async Task<MarcacionAsistenciaDto>
            RegistrarAutomaticoAsync(
                RegistrarMarcacionAutomaticaDto dto)
        {
            var response =
                await _httpClient
                    .PostAsJsonAsync(
                        "api/marcaciones/automatico",
                        dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<MarcacionAsistenciaDto>()
                ??
                throw new InvalidOperationException(
                    "La API no devolvió la marcación registrada.");
        }


        public async Task<List<MarcacionAsistenciaDto>>
            RegistrarManualAsync(
                RegistrarAsistenciaManualDto dto)
        {
            var response =
                await _httpClient
                    .PostAsJsonAsync(
                        "api/marcaciones/manual",
                        dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<List<MarcacionAsistenciaDto>>()
                ??
                new List<MarcacionAsistenciaDto>();
        }


        public async Task<List<MarcacionAsistenciaDto>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha)
        {
            string fechaTexto =
                fecha.ToString(
                    "yyyy-MM-dd");


            var response =
                await _httpClient.GetAsync(
                    $"api/marcaciones/empleado/{empleadoId}" +
                    $"?fecha={fechaTexto}");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<List<MarcacionAsistenciaDto>>()
                ??
                new List<MarcacionAsistenciaDto>();
        }


        public async Task<List<AsistenciaDiariaDto>>
            ObtenerResumenDiarioAsync(
                int negocioId,
                DateTime fecha)
        {
            string fechaTexto =
                fecha.ToString(
                    "yyyy-MM-dd");


            var response =
                await _httpClient.GetAsync(
                    $"api/marcaciones/resumen" +
                    $"?negocioId={negocioId}" +
                    $"&fecha={fechaTexto}");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<List<AsistenciaDiariaDto>>()
                ??
                new List<AsistenciaDiariaDto>();
        }


        private static async Task<string>
            ObtenerMensajeErrorAsync(
                HttpResponseMessage response)
        {
            var mensaje =
                await response.Content
                    .ReadAsStringAsync();


            if (string.IsNullOrWhiteSpace(
                mensaje))
            {
                return
                    $"Error {(int)response.StatusCode}.";
            }


            return mensaje.Trim(
                '"');
        }
    }
}