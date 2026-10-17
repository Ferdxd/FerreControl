using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IEmpleadoCredencialRepository
    {
        Task<List<EmpleadoCredencial>>ObtenerPorEmpleadoAsync(int empleadoId);

        Task<EmpleadoCredencial?>ObtenerPorIdAsync(int id);

        Task<EmpleadoCredencial?>ObtenerActivaPorCodigoAsync(string tipoCredencial,string codigo);

        Task<bool> ExisteCodigoAsync(string tipoCredencial,string codigo);

        Task AgregarAsync(EmpleadoCredencial credencial);

        Task ActualizarAsync(EmpleadoCredencial credencial);
    }
}