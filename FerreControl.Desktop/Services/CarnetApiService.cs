using FerreControl.Application.DTOs.EmpleadoCredencial;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class CarnetApiService
    {
        private readonly HttpClient
            _httpClient;


        public CarnetApiService()
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
        // CARNÉS
        // =====================================================

        public async Task<List<CarnetEmpleadoDto>>
            ObtenerCarnesAsync(
                int negocioId)
        {
            var response =
                await _httpClient
                    .GetAsync(
                        $"api/carnes?negocioId={negocioId}");


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    List<CarnetEmpleadoDto>>()
                ??
                new List<CarnetEmpleadoDto>();
        }


        // =====================================================
        // GENERAR
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            GenerarAsync(
                int empleadoId)
        {
            var response =
                await _httpClient
                    .PostAsync(
                        $"api/carnes/empleados/{empleadoId}/generar",
                        null);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    EmpleadoCredencialDto>();
        }


        // =====================================================
        // REEMPLAZAR
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            ReemplazarAsync(
                int empleadoId)
        {
            var response =
                await _httpClient
                    .PostAsync(
                        $"api/carnes/empleados/{empleadoId}/reemplazar",
                        null);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    EmpleadoCredencialDto>();
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            DesactivarAsync(
                int empleadoId)
        {
            var response =
                await _httpClient
                    .PutAsync(
                        $"api/carnes/empleados/{empleadoId}/desactivar",
                        null);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    EmpleadoCredencialDto>();
        }


        // =====================================================
        // GENERAR FALTANTES
        // =====================================================

        public async Task<List<EmpleadoCredencialDto>>
            GenerarFaltantesAsync(
                int negocioId)
        {
            var response =
                await _httpClient
                    .PostAsync(
                        $"api/carnes/generar-faltantes?negocioId={negocioId}",
                        null);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    List<EmpleadoCredencialDto>>()
                ??
                new List<EmpleadoCredencialDto>();
        }


        // =====================================================
        // TODAS LAS CREDENCIALES
        // =====================================================

        public async Task<List<EmpleadoCredencialDto>>
            ObtenerCredencialesAsync(
                int empleadoId)
        {
            var response =
                await _httpClient
                    .GetAsync(
                        $"api/empleados/{empleadoId}/credenciales");


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    List<EmpleadoCredencialDto>>()
                ??
                new List<EmpleadoCredencialDto>();
        }


        // =====================================================
        // CREDENCIAL MANUAL
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            CrearManualAsync(
                int empleadoId,
                CrearEmpleadoCredencialDto dto)
        {
            var response =
                await _httpClient
                    .PostAsJsonAsync(
                        $"api/empleados/{empleadoId}/credenciales",
                        dto);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    EmpleadoCredencialDto>();
        }


        // =====================================================
        // CAMBIAR ESTADO
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            ActualizarEstadoAsync(
                int empleadoId,
                int credencialId,
                bool estado)
        {
            var dto =
                new ActualizarEstadoCredencialDto
                {
                    Estado =
                        estado
                };


            var response =
                await _httpClient
                    .PutAsJsonAsync(
                        $"api/empleados/{empleadoId}/credenciales/{credencialId}/estado",
                        dto);


            await ValidarRespuestaAsync(
                response);


            return await response.Content
                .ReadFromJsonAsync<
                    EmpleadoCredencialDto>();
        }


        // =====================================================
        // ERRORES
        // =====================================================

        private static async Task
            ValidarRespuestaAsync(
                HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;


            var contenido =
                await response.Content
                    .ReadAsStringAsync();


            contenido =
                contenido
                    .Trim()
                    .Trim('"');


            if (string.IsNullOrWhiteSpace(
                contenido))
            {
                contenido =
                    $"Error {(int)response.StatusCode}: {response.ReasonPhrase}";
            }


            throw new InvalidOperationException(
                contenido);
        }
    }
}