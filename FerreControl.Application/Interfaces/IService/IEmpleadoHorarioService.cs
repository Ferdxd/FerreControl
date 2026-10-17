using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Application.DTOs.EmpleadoHorario;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IEmpleadoHorarioService
    {
        Task<List<EmpleadoHorarioDto>> ObtenerHistorialAsync(
            int empleadoId);

        Task<EmpleadoHorarioDto?> ObtenerActualAsync(
            int empleadoId);

        Task<EmpleadoHorarioDto> AsignarHorarioAsync(
            int empleadoId,
            CrearEmpleadoHorarioDto dto);
    }
}