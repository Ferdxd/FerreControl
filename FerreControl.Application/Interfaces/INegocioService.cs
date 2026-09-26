using FerreControl.Application.DTOs.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces
{
    public interface INegocioService
    {
        Task<List<NegocioDto>> ObtenerTodosAsync();
        Task<NegocioDto?> ObtenerPorIdAsync(int id);
        Task<NegocioDto> CrearNegocio(CrearNegocioDto dto);
        Task<NegocioDto> ActualizarNegocio(int id,ActualizarNegocioDto dto);
    }
}
