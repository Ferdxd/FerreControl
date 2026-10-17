
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class EmpleadoCredencialRepository
        : IEmpleadoCredencialRepository
    {
        private readonly FerreControlDbContext _db;

        public EmpleadoCredencialRepository(
            FerreControlDbContext db)
        {
            _db = db;
        }


        public async Task<List<EmpleadoCredencial>>
            ObtenerPorEmpleadoAsync(int empleadoId)
        {
            return await _db.EmpleadosCredenciales
                .AsNoTracking()
                .Where(x =>
                    x.EmpleadoID == empleadoId)
                .OrderBy(x => x.TipoCredencial)
                .ToListAsync();
        }


        public async Task<EmpleadoCredencial?>
            ObtenerPorIdAsync(int id)
        {
            return await _db.EmpleadosCredenciales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);
        }


        public async Task<EmpleadoCredencial?>
            ObtenerActivaPorCodigoAsync(
                string tipoCredencial,
                string codigo)
        {
            return await _db.EmpleadosCredenciales
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TipoCredencial ==
                        tipoCredencial &&
                    x.Codigo == codigo &&
                    x.Estado);
        }


        public async Task<bool> ExisteCodigoAsync(
            string tipoCredencial,
            string codigo)
        {
            return await _db.EmpleadosCredenciales
                .AnyAsync(x =>
                    x.TipoCredencial ==
                        tipoCredencial &&
                    x.Codigo == codigo);
        }


        public async Task AgregarAsync(
            EmpleadoCredencial credencial)
        {
            await _db.EmpleadosCredenciales
                .AddAsync(credencial);

            await _db.SaveChangesAsync();
        }


        public async Task ActualizarAsync(
            EmpleadoCredencial credencial)
        {
            _db.EmpleadosCredenciales
                .Update(credencial);

            await _db.SaveChangesAsync();
        }
    }
}