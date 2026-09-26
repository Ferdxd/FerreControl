using FerreControl.Application.DTOs.Salarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces
{
    public interface ISalarioEmpleadoService
    {
        Task<List<SalarioEmpleadoDto>> ObtenerHistorialAsync(int idEmpleado);
        Task<SalarioEmpleadoDto?> ObtenerActualAsync(int idEmpleado);
        Task<SalarioEmpleadoDto> RegistrarSalarioAsync(int idEmpleado, CrearSalarioEmpleadoDto dto);
        
    }
}
