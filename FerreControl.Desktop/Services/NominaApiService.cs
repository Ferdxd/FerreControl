using FerreControl.Application.DTOs.Nomina;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class NominaApiService
    {
        private readonly HttpClient _httpClient;


        public NominaApiService()
        {
            _httpClient =
                new HttpClient
                {
                    BaseAddress =
                        new Uri(
                            "https://localhost:7217/")
                };
        }


        // =====================================================
        // NÓMINAS
        // =====================================================

        public async Task<List<NominaDto>>
            ObtenerPorNegocioAsync(
                int negocioId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Nominas/negocio/{negocioId}");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<List<NominaDto>>()
                ??
                new List<NominaDto>();
        }


        public async Task<NominaDto?>
            ObtenerPorIdAsync(
                int id)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/Nominas/{id}");


            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return null;
            }


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<NominaDto>();
        }


        public async Task<NominaDto>
            CrearAsync(
                CrearNominaDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Nominas",
                    dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            var resultado =
                await response.Content
                    .ReadFromJsonAsync<NominaDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió la nómina creada.");
            }


            return resultado;
        }


        public async Task<NominaDto>
            ActualizarEstadoAsync(
                int nominaId,
                string estado)
        {
            var dto =
                new ActualizarEstadoNominaDto
                {
                    Estado =
                        estado
                };


            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Nominas/{nominaId}/estado",
                    dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            var resultado =
                await response.Content
                    .ReadFromJsonAsync<NominaDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió la nómina actualizada.");
            }


            return resultado;
        }


        // =====================================================
        // DETALLE
        // =====================================================

        public async Task<List<NominaDetalleDto>>
            ObtenerDetallesAsync(
                int nominaId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/nominas/{nominaId}/detalles");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<NominaDetalleDto>>()
                ??
                new List<NominaDetalleDto>();
        }


        public async Task<NominaDetalleDto>
            AgregarDetalleAsync(
                int nominaId,
                CrearNominaDetalleDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/nominas/{nominaId}/detalles",
                    dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            var resultado =
                await response.Content
                    .ReadFromJsonAsync<
                        NominaDetalleDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió el detalle creado.");
            }


            return resultado;
        }


        // =====================================================
        // ERRORES
        // =====================================================

        private static async Task<string>
            ObtenerErrorAsync(
                HttpResponseMessage response)
        {
            var contenido =
                await response.Content
                    .ReadAsStringAsync();


            if (string.IsNullOrWhiteSpace(
                contenido))
            {
                return
                    $"Error HTTP {(int)response.StatusCode}.";
            }


            return contenido;
        }
    }
}