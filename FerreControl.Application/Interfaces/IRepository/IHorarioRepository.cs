using FerreControl.Application.DTOs.Horarios;
using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IHorarioRepository
    {
        Task<List<Horario>> ObtenerHorarioPorNegocioAsync(int idNegocio);
        Task<Horario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Horario horario);
        Task ActualizarAsync(Horario horario);
    }
}
