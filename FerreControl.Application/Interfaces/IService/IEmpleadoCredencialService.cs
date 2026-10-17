using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.DTOs.EmpleadoCredencial;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IEmpleadoCredencialService
    {
        Task<List<EmpleadoCredencialDto>>
            ObtenerPorEmpleadoAsync(int empleadoId);

        Task<EmpleadoCredencialDto> CrearAsync(
            int empleadoId,
            CrearEmpleadoCredencialDto dto);

        Task<EmpleadoCredencialDto?>
            ActualizarEstadoAsync(
                int credencialId,
                bool estado);
    }
}