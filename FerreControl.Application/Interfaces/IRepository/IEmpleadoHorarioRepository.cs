using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IEmpleadoHorarioRepository
    {
        Task<List<EmpleadoHorario>> ObtenerHistorialAsync(
            int empleadoId);

        Task<EmpleadoHorario?> ObtenerActualAsync(
            int empleadoId);

        Task<EmpleadoHorario?> ObtenerPorIdAsync(
            int id);

        Task<EmpleadoHorario?> ObtenerVigenteEnFechaAsync(
            int empleadoId,
            DateTime fecha);

        Task AgregarAsync(
            EmpleadoHorario empleadoHorario);

        Task ActualizarAsync(
            EmpleadoHorario empleadoHorario);
    }
}
