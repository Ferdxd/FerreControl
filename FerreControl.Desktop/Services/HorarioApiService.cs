using FerreControl.Application.DTOs.Horarios;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class HorarioApiService
    {
        private readonly HttpClient _httpClient;

        public HorarioApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7217/")
            };
        }


        public async Task<List<HorarioDto>> ObtenerPorNegocioAsync(
            int negocioId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Horarios/negocio/{negocioId}");

            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    $"Error {(int)response.StatusCode}: {mensaje}");
            }

            return await response.Content
                .ReadFromJsonAsync<List<HorarioDto>>()
                ?? new List<HorarioDto>();
        }


        public async Task<HorarioDto?> ObtenerPorIdAsync(
            int id)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Horarios/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<HorarioDto>();
        }


        public async Task<HorarioDto?> CrearAsync(
            CrearHorarioDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Horarios",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<HorarioDto>();
        }


        public async Task<HorarioDto?> ActualizarAsync(
            int id,
            ActualizarHorarioDto dto)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Horarios/{id}",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<HorarioDto>();
        }
    }
}