using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class PagoNominaRepository
        : IPagoNominaRepository
    {
        private readonly FerreControlDbContext _context;

        public PagoNominaRepository(
            FerreControlDbContext context)
        {
            _context = context;
        }


        public async Task<List<PagoNomina>>
            ObtenerPorDetalleAsync(
                int nominaDetalleId)
        {
            return await _context.PagosNomina
                .Where(x =>
                    x.NominaDetalleID == nominaDetalleId)
                .OrderByDescending(x => x.FechaPago)
                .ToListAsync();
        }


        public async Task<PagoNomina?> ObtenerPorIdAsync(
            int id)
        {
            return await _context.PagosNomina
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task<long>
            ObtenerTotalPagadoCentavosAsync(
                int nominaDetalleId)
        {
            return await _context.PagosNomina
                .Where(x =>
                    x.NominaDetalleID == nominaDetalleId &&
                    x.Estado)
                .SumAsync(x => x.MontoCentavos);
        }


        public async Task AgregarAsync(
            PagoNomina pago)
        {
            await _context.PagosNomina
                .AddAsync(pago);

            await _context.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            PagoNomina pago)
        {
            _context.PagosNomina.Update(pago);

            await _context.SaveChangesAsync();
        }
    }
}