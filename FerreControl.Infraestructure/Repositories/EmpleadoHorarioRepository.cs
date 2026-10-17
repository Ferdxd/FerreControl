using System;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class EmpleadoHorarioRepository
        : IEmpleadoHorarioRepository
    {
        private readonly FerreControlDbContext _db;

        public EmpleadoHorarioRepository(
            FerreControlDbContext db)
        {
            _db = db;
        }


        public async Task<List<EmpleadoHorario>>
            ObtenerHistorialAsync(int empleadoId)
        {
            return await _db.EmpleadosHorarios
                .AsNoTracking()
                .Include(x => x.Horario)
                .Where(x => x.EmpleadoID == empleadoId)
                .OrderByDescending(x => x.FechaInicio)
                .ToListAsync();
        }


        public async Task<EmpleadoHorario?>
            ObtenerActualAsync(int empleadoId)
        {
            return await _db.EmpleadosHorarios
                .AsNoTracking()
                .Include(x => x.Horario)
                .FirstOrDefaultAsync(x =>
                    x.EmpleadoID == empleadoId &&
                    x.Estado &&
                    x.FechaFin == null);
        }


        public async Task<EmpleadoHorario?>
            ObtenerPorIdAsync(int id)
        {
            return await _db.EmpleadosHorarios
                .AsNoTracking()
                .Include(x => x.Horario)
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task<EmpleadoHorario?>
            ObtenerVigenteEnFechaAsync(
                int empleadoId,
                DateTime fecha)
        {
            var dia = fecha.Date;

            return await _db.EmpleadosHorarios
                .AsNoTracking()
                .Include(x => x.Horario)
                .FirstOrDefaultAsync(x =>
                    x.EmpleadoID == empleadoId &&
                    x.FechaInicio <= dia &&
                    (x.FechaFin == null ||
                     x.FechaFin >= dia));
        }


        public async Task AgregarAsync(
            EmpleadoHorario empleadoHorario)
        {
            await _db.EmpleadosHorarios
                .AddAsync(empleadoHorario);

            await _db.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            EmpleadoHorario empleadoHorario)
        {
            _db.EmpleadosHorarios.Update(
                empleadoHorario);

            await _db.SaveChangesAsync();
        }
    }
}