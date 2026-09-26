using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces
{
    public interface INegocioRepository
    {
        Task<List<Negocio>> ObtenerTodosAsync();
        Task<Negocio?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Negocio negocio);
        Task ActualizarAsync(Negocio negocio);
    }
}
