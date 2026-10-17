using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class NominaRepository : INominaRepository
    {
        private readonly FerreControlDbContext _context;

        public NominaRepository(
            FerreControlDbContext context)
        {
            _context = context;
        }


        public async Task<List<Nomina>> ObtenerPorNegocioAsync(
            int negocioId)
        {
            return await _context.Nominas
                .Where(x => x.NegocioID == negocioId)
                .OrderByDescending(x => x.FechaInicioPeriodo)
                .ToListAsync();
        }


        public async Task<Nomina?> ObtenerPorIdAsync(int id)
        {
            return await _context.Nominas
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task AgregarAsync(Nomina nomina)
        {
            await _context.Nominas.AddAsync(nomina);

            await _context.SaveChangesAsync();
        }


        public async Task ActualizarAsync(Nomina nomina)
        {
            _context.Nominas.Update(nomina);

            await _context.SaveChangesAsync();
        }
    }
}