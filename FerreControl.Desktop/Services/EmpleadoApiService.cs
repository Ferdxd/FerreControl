using FerreControl.Application.DTOs.Empleados;
using FerreControl.Desktop.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FerreControl.Desktop.Services
{
    public class EmpleadoApiService
    {
        private readonly HttpClient _httpClient;
        public EmpleadoApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7217/")
            };
        }

        public async Task<List<EmpleadoDto>> GetObtenerTodos()
        {
            var empleados= await _httpClient.GetFromJsonAsync<List<EmpleadoDto>>("api/Empleados");
            return empleados?? new List<EmpleadoDto> ();
        }

        public async Task<EmpleadoDto?> ObtenerPorId(int id)
        {
            var empleado = await _httpClient.GetFromJsonAsync<EmpleadoDto>($"api/Empleados/{id}");
            return empleado;
        }

        public async Task<EmpleadoDto?> PostCrearEmpleado(CrearEmpleadoDto dto)
        {
            var response=await _httpClient.PostAsJsonAsync("api/Empleados", dto);
            response.EnsureSuccessStatusCode(); 

            return await response.Content.ReadFromJsonAsync<EmpleadoDto>();
        }
        
        public async  Task<EmpleadoDto?> ActualizarAsync(int id, ActualizarEmpleadoDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Empleados/{id}", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EmpleadoDto>();
        }
        



    }
}
