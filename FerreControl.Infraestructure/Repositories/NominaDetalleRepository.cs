using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class NominaDetalleRepository
        : INominaDetalleRepository
    {
        private readonly FerreControlDbContext _context;

        public NominaDetalleRepository(
            FerreControlDbContext context)
        {
            _context = context;
        }


        public async Task<List<NominaDetalle>>
            ObtenerPorNominaAsync(int nominaId)
        {
            return await _context.NominasDetalle
                .Where(x => x.NominaID == nominaId)
                .OrderBy(x => x.EmpleadoID)
                .ToListAsync();
        }


        public async Task<NominaDetalle?> ObtenerPorIdAsync(
            int id)
        {
            return await _context.NominasDetalle
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task<NominaDetalle?>
            ObtenerPorNominaEmpleadoAsync(
                int nominaId,
                int empleadoId)
        {
            return await _context.NominasDetalle
                .FirstOrDefaultAsync(x =>
                    x.NominaID == nominaId &&
                    x.EmpleadoID == empleadoId);
        }


        public async Task AgregarAsync(
            NominaDetalle detalle)
        {
            await _context.NominasDetalle
                .AddAsync(detalle);

            await _context.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            NominaDetalle detalle)
        {
            _context.NominasDetalle.Update(detalle);

            await _context.SaveChangesAsync();
        }
    }
}