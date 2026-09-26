using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces
{
    public interface ISalarioEmpleadoRepository
    {
        Task<List<SalarioEmpleado>> ObtenerHistorialAsync(int idEmpleado);
        Task<SalarioEmpleado?> ObtenerActualAsync(int idEmpleado);
        Task AgregarAsync(SalarioEmpleado salario);
        Task ActualizarAsync(SalarioEmpleado salario);

       
    }
}
