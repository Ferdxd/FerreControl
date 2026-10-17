using FerreControl.Application.DTOs.Horarios;
using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IHorarioService
    {
        Task<HorarioDto> CrearHorarioAsync(CrearHorarioDto crearHorarioDto);
        Task<HorarioDto?> ObtenerHorarioPorIdAsync(int id);
        Task<IEnumerable<HorarioDto>> ObtenerHorariosPorNegocioIdAsync(int negocioId);
        Task<HorarioDto> ActualizarHorarioAsync(int id, ActualizarHorarioDto actualizarHorarioDto);
    }
}
