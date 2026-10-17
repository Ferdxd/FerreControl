using FerreControl.Application.DTOs.HorarioDetalle;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class DetalleHorarioApiService
    {
        private readonly HttpClient _httpClient;

        public DetalleHorarioApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7217/")
            };
        }


        public async Task<List<HorarioDetalleDto>> ObtenerPorHorarioAsync(
            int horarioId)
        {
            var response = await _httpClient.GetAsync(
                $"api/HorarioDetalle/horario/{horarioId}");

            if (!response.IsSuccessStatusCode)
            {
                var mensaje = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<List<HorarioDetalleDto>>()
                ?? new List<HorarioDetalleDto>();
        }


        public async Task<HorarioDetalleDto?> CrearAsync(
            int horarioId,
            CrearHorarioDetalleDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/HorarioDetalle/horario/{horarioId}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<HorarioDetalleDto>();
        }


        public async Task<HorarioDetalleDto?> ActualizarAsync(
            int id,
            ActualizarHorarioDetalleDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/HorarioDetalle/{id}",
                dto);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(mensaje);
            }

            return await response.Content
                .ReadFromJsonAsync<HorarioDetalleDto>();
        }
    }
}