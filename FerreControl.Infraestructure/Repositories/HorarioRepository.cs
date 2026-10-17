using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Infraestructure.Repositories
{
    public class HorarioRepository : IHorarioRepository
    {
        private readonly FerreControlDbContext _db;

        public HorarioRepository(FerreControlDbContext db)
        {
            _db = db;
        }
        public async Task ActualizarAsync(Horario horario)
        {
            _db.Horarios.Update(horario);
            await _db.SaveChangesAsync();
        }

        public async Task AgregarAsync(Horario horario)
        {
            _db.Horarios.Add(horario);
            await _db.SaveChangesAsync();
        }

        public async Task<Horario?> ObtenerPorIdAsync(int id)
        {
            return await _db.Horarios.AsNoTracking().
                FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<List<Horario>> ObtenerHorarioPorNegocioAsync(int id)
        {
            return await _db.Horarios.AsNoTracking()
               .Where(x => x.NegocioID == id)
               .OrderBy(x => x.Nombre)
               .ToListAsync();
        }

    }
}
