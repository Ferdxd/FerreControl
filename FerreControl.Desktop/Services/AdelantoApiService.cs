using FerreControl.Application.DTOs.Adelantos;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class AdelantoApiService
    {
        private readonly HttpClient _httpClient;


        public AdelantoApiService()
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
        // ADELANTOS
        // =====================================================

        public async Task<List<AdelantoEmpleadoDto>>
            ObtenerPorEmpleadoAsync(
                int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/adelantos");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<AdelantoEmpleadoDto>>()
                ??
                new List<AdelantoEmpleadoDto>();
        }


        public async Task<List<AdelantoEmpleadoDto>>
            ObtenerPendientesPorEmpleadoAsync(
                int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/adelantos/pendientes");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<AdelantoEmpleadoDto>>()
                ??
                new List<AdelantoEmpleadoDto>();
        }


        public async Task<AdelantoEmpleadoDto?>
            ObtenerPorIdAsync(
                int id)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/adelantos/{id}");


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
                .ReadFromJsonAsync<
                    AdelantoEmpleadoDto>();
        }


        public async Task<AdelantoEmpleadoDto>
            CrearAsync(
                int empleadoId,
                CrearAdelantoEmpleadoDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/empleados/{empleadoId}/adelantos",
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
                        AdelantoEmpleadoDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió el adelanto creado.");
            }


            return resultado;
        }


        public async Task<AdelantoEmpleadoDto>
            AnularAsync(
                int adelantoId,
                AnularAdelantoEmpleadoDto dto)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/adelantos/{adelantoId}/anular",
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
                        AdelantoEmpleadoDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió el adelanto actualizado.");
            }


            return resultado;
        }


        // =====================================================
        // APLICACIONES
        // =====================================================

        public async Task<List<AdelantoAplicacionDto>>
            ObtenerAplicacionesPorAdelantoAsync(
                int adelantoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/adelantos/{adelantoId}/aplicaciones");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<AdelantoAplicacionDto>>()
                ??
                new List<AdelantoAplicacionDto>();
        }


        public async Task<List<AdelantoAplicacionDto>>
            ObtenerAplicacionesPorNominaDetalleAsync(
                int nominaDetalleId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/nomina-detalles/{nominaDetalleId}/adelantos");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<AdelantoAplicacionDto>>()
                ??
                new List<AdelantoAplicacionDto>();
        }


        public async Task<AdelantoAplicacionDto>
            AplicarAsync(
                int adelantoId,
                CrearAdelantoAplicacionDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/adelantos/{adelantoId}/aplicaciones",
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
                        AdelantoAplicacionDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió la aplicación creada.");
            }


            return resultado;
        }


        public async Task<AdelantoAplicacionDto>
            AnularAplicacionAsync(
                int aplicacionId,
                AnularAdelantoAplicacionDto dto)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/adelanto-aplicaciones/{aplicacionId}/anular",
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
                        AdelantoAplicacionDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió la aplicación actualizada.");
            }


            return resultado;
        }


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