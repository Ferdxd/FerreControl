using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Repositories
{
    public class SalarioEmpleadoRepository : ISalarioEmpleadoRepository
    {
        private readonly FerreControlDbContext _db;

        public SalarioEmpleadoRepository(
            FerreControlDbContext db)
        {
            _db = db;
        }


        public async Task<List<SalarioEmpleado>> ObtenerHistorialAsync(
            int idEmpleado)
        {
            return await _db.SalariosEmpleado
                .AsNoTracking()
                .Where(x => x.EmpleadoID == idEmpleado)
                .OrderByDescending(x => x.FechaInicio)
                .ToListAsync();
        }


        public async Task<SalarioEmpleado?> ObtenerActualAsync(
            int idEmpleado)
        {
            return await _db.SalariosEmpleado
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EmpleadoID == idEmpleado &&
                    x.Estado == true &&
                    x.FechaFin == null);
        }


        public async Task AgregarAsync(
            SalarioEmpleado salario)
        {
            await _db.SalariosEmpleado
                .AddAsync(salario);

            await _db.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            SalarioEmpleado salario)
        {
            _db.SalariosEmpleado.Update(salario);

            await _db.SaveChangesAsync();
        }
    }
}