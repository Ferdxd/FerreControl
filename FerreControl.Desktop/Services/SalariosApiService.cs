using FerreControl.Application.DTOs.Salarios;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class SalariosApiService
    {
        private readonly HttpClient _httpClient;

        public SalariosApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "https://localhost:7217/")
            };
        }


        // =========================================================
        // HISTORIAL
        // =========================================================

        public async Task<List<SalarioEmpleadoDto>>
            ObtenerHistorialAsync(int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/salarios");


            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    mensaje);
            }


            return await response.Content
                .ReadFromJsonAsync<List<SalarioEmpleadoDto>>()
                ?? new List<SalarioEmpleadoDto>();
        }


        // =========================================================
        // SALARIO ACTUAL
        // =========================================================

        public async Task<SalarioEmpleadoDto?>
            ObtenerActualAsync(int empleadoId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/empleados/{empleadoId}/salarios/actual");


            if (response.StatusCode ==
                HttpStatusCode.NotFound ||
                response.StatusCode ==
                HttpStatusCode.NoContent)
            {
                return null;
            }


            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    mensaje);
            }


            return await response.Content
                .ReadFromJsonAsync<SalarioEmpleadoDto>();
        }


        // =========================================================
        // REGISTRAR / CAMBIAR
        // =========================================================

        public async Task<SalarioEmpleadoDto?>
            RegistrarAsync(
                int empleadoId,
                CrearSalarioEmpleadoDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/empleados/{empleadoId}/salarios",
                    dto);


            if (!response.IsSuccessStatusCode)
            {
                var mensaje =
                    await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    mensaje.Replace("\"", ""));
            }


            return await response.Content
                .ReadFromJsonAsync<SalarioEmpleadoDto>();
        }
    }
}