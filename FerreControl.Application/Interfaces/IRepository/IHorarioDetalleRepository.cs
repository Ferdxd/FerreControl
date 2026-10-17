using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IHorarioDetalleRepository
    {
        Task<List<HorarioDetalle>> ObtenerPorHorarioAsync(
            int horarioId);

        Task<HorarioDetalle?> ObtenerPorIdAsync(
            int id);

        Task<bool> ExisteDiaAsync(
            int horarioId,
            int diaSemana,
            int? excluirId = null);

        Task AgregarAsync(
            HorarioDetalle detalle);

        Task ActualizarAsync(
            HorarioDetalle detalle);
    }
}