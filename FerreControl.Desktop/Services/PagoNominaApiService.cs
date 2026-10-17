using FerreControl.Application.DTOs.Nomina;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class PagoNominaApiService
    {
        private readonly HttpClient _httpClient;


        public PagoNominaApiService()
        {
            _httpClient =
                new HttpClient
                {
                    BaseAddress =
                        new Uri(
                            "https://localhost:7217/")
                };
        }


        public async Task<List<PagoNominaDto>>
            ObtenerPorDetalleAsync(
                int nominaDetalleId)
        {
            var response =
                await _httpClient.GetAsync(
                    $"api/nomina-detalles/{nominaDetalleId}/pagos");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            return await response.Content
                .ReadFromJsonAsync<
                    List<PagoNominaDto>>()
                ??
                new List<PagoNominaDto>();
        }


        public async Task<PagoNominaDto>
            RegistrarAsync(
                int nominaDetalleId,
                CrearPagoNominaDto dto)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"api/nomina-detalles/{nominaDetalleId}/pagos",
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
                        PagoNominaDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió el pago registrado.");
            }


            return resultado;
        }


        public async Task<PagoNominaDto>
            AnularAsync(
                int nominaDetalleId,
                int pagoId)
        {
            var response =
                await _httpClient.PutAsync(
                    $"api/nomina-detalles/{nominaDetalleId}/pagos/{pagoId}/anular",
                    null);


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    await ObtenerErrorAsync(
                        response));
            }


            var resultado =
                await response.Content
                    .ReadFromJsonAsync<
                        PagoNominaDto>();


            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "La API no devolvió el pago actualizado.");
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