using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Infraestructure.Repositories
{
    public class NegocioRepository : INegocioRepository
    {
        private readonly FerreControlDbContext _db;
        public NegocioRepository(FerreControlDbContext db)
        {
            _db = db;
        }
        public async Task ActualizarAsync(Negocio negocio)
        {
            _db.Negocios.Update(negocio);
            await _db.SaveChangesAsync();
        }

        public async Task AgregarAsync(Negocio negocio)
        {
            await _db.Negocios.AddAsync(negocio);
            await _db.SaveChangesAsync();
        }

        public async Task<Negocio?> ObtenerPorIdAsync(int id)
        {
            return await _db.Negocios
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<List<Negocio>> ObtenerTodosAsync()
        {
            return await _db.Negocios
                .AsNoTracking()
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }
    }
}
