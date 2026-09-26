using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FerreControl.Application.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<List<Empleado>> ObtenerTodosAsync();

        Task<Empleado?> ObtenerPorIdAsync(int id);

        Task<bool> ExisteCodigoAsync(
            int negocioID,
            string codigo,
            int? empleadoIDExcluir = null);

        Task AgregarAsync(Empleado empleado);

        Task ActualizarAsync(Empleado empleado);
    }
}
