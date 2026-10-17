using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.Interfaces;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Repositories
{
    public class HorarioDetalleRepository
        : IHorarioDetalleRepository
    {
        private readonly FerreControlDbContext _db;


        public HorarioDetalleRepository(
            FerreControlDbContext db)
        {
            _db = db;
        }


        public async Task<List<HorarioDetalle>>
            ObtenerPorHorarioAsync(int horarioId)
        {
            return await _db.HorariosDetalle
                .AsNoTracking()
                .Where(x => x.HorarioID == horarioId)
                .OrderBy(x => x.DiaSemana)
                .ToListAsync();
        }


        public async Task<HorarioDetalle?> ObtenerPorIdAsync(
            int id)
        {
            return await _db.HorariosDetalle
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task<bool> ExisteDiaAsync(
            int horarioId,
            int diaSemana,
            int? excluirId = null)
        {
            return await _db.HorariosDetalle
                .AnyAsync(x =>
                    x.HorarioID == horarioId &&
                    x.DiaSemana == diaSemana &&
                    (!excluirId.HasValue ||
                     x.ID != excluirId.Value));
        }


        public async Task AgregarAsync(
            HorarioDetalle detalle)
        {
            await _db.HorariosDetalle
                .AddAsync(detalle);

            await _db.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            HorarioDetalle detalle)
        {
            _db.HorariosDetalle.Update(detalle);

            await _db.SaveChangesAsync();
        }
    }
}