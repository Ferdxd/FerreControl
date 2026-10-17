using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.DTOs.HorarioDetalle;


namespace FerreControl.Application.Interfaces.IService 
{ 

    public interface IHorarioDetalleService
    {
        Task<List<HorarioDetalleDto>> ObtenerPorHorarioAsync(
            int horarioId);

        Task<HorarioDetalleDto?> ObtenerPorIdAsync(
            int id);

        Task<HorarioDetalleDto> CrearAsync(
            int horarioId,
            CrearHorarioDetalleDto dto);

        Task<HorarioDetalleDto?> ActualizarAsync(
            int id,
            ActualizarHorarioDetalleDto dto);
    }
}
