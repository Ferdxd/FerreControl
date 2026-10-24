using FerreControl.Application.DTOs.EmpleadoHorario;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class EmpleadoHorarioApiService
    {
        private readonly HttpClient
            _httpClient;


        public EmpleadoHorarioApiService()
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
        // HORARIO ACTUAL
        // =====================================================

        public async Task<EmpleadoHorarioDto?>ObtenerActualAsync(int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/horarios/actual");


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<EmpleadoHorarioDto>();
        }


        // =====================================================
        // HISTORIAL
        // =====================================================

        public async Task<List<EmpleadoHorarioDto>> ObtenerHistorialAsync(int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/horarios");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<List<EmpleadoHorarioDto>>()
                ??
                new List<EmpleadoHorarioDto>();
        }


        // =====================================================
        // ASIGNAR / CAMBIAR HORARIO
        // =====================================================

        public async Task<EmpleadoHorarioDto?>AsignarAsync(int empleadoId,CrearEmpleadoHorarioDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/empleados/{empleadoId}/horarios",
                    dto);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerMensajeErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<EmpleadoHorarioDto>();
        }


        // =====================================================
        // ERROR API
        // =====================================================

        private static async Task<string> ObtenerMensajeErrorAsync(HttpResponseMessage response)
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


            return mensaje
                .Trim()
                .Trim('"');
        }
    }
}