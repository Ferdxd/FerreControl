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


        // =====================================================
        // POR EMPLEADO
        // =====================================================

        public async Task<List<EmpleadoCredencial>>
            ObtenerPorEmpleadoAsync(
                int empleadoId)
        {
            return await _db
                .EmpleadosCredenciales
                .AsNoTracking()
                .Where(x =>
                    x.EmpleadoID ==
                    empleadoId)
                .OrderByDescending(x =>
                    x.Estado)
                .ThenBy(x =>
                    x.TipoCredencial)
                .ThenByDescending(x =>
                    x.FechaCreacion)
                .ToListAsync();
        }


        // =====================================================
        // POR ID
        // =====================================================

        public async Task<EmpleadoCredencial?>
            ObtenerPorIdAsync(
                int id)
        {
            return await _db
                .EmpleadosCredenciales
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.ID == id);
        }


        // =====================================================
        // ACTIVA POR CÓDIGO
        // =====================================================

        public async Task<EmpleadoCredencial?>
            ObtenerActivaPorCodigoAsync(
                string tipoCredencial,
                string codigo)
        {
            return await _db
                .EmpleadosCredenciales
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TipoCredencial ==
                    tipoCredencial
                    &&
                    x.Codigo ==
                    codigo
                    &&
                    x.Estado);
        }


        // =====================================================
        // ACTIVA POR EMPLEADO Y TIPO
        // =====================================================

        public async Task<EmpleadoCredencial?>
            ObtenerActivaPorEmpleadoYTipoAsync(
                int empleadoId,
                string tipoCredencial)
        {
            return await _db
                .EmpleadosCredenciales
                .AsNoTracking()
                .Where(x =>
                    x.EmpleadoID ==
                    empleadoId
                    &&
                    x.TipoCredencial ==
                    tipoCredencial
                    &&
                    x.Estado)
                .OrderByDescending(x =>
                    x.FechaCreacion)
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // ACTIVAS DEL NEGOCIO
        // =====================================================

        public async Task<List<EmpleadoCredencial>>
            ObtenerActivasPorNegocioYTipoAsync(
                int negocioId,
                string tipoCredencial)
        {
            return await _db
                .EmpleadosCredenciales
                .Include(x =>
                    x.Empleado)
                .AsNoTracking()
                .Where(x =>
                    x.Estado
                    &&
                    x.TipoCredencial ==
                    tipoCredencial
                    &&
                    x.Empleado.NegocioID ==
                    negocioId
                    &&
                    x.Empleado.Estado)
                .OrderByDescending(x =>
                    x.FechaCreacion)
                .ToListAsync();
        }


        // =====================================================
        // EXISTENCIA
        // =====================================================

        public async Task<bool>
            ExisteCodigoAsync(
                string tipoCredencial,
                string codigo)
        {
            return await _db
                .EmpleadosCredenciales
                .AnyAsync(x =>
                    x.TipoCredencial ==
                    tipoCredencial
                    &&
                    x.Codigo ==
                    codigo);
        }


        // =====================================================
        // AGREGAR
        // =====================================================

        public async Task AgregarAsync(
            EmpleadoCredencial credencial)
        {
            await _db
                .EmpleadosCredenciales
                .AddAsync(
                    credencial);


            await _db
                .SaveChangesAsync();
        }


        // =====================================================
        // ACTUALIZAR
        // =====================================================

        public async Task ActualizarAsync(
            EmpleadoCredencial credencial)
        {
            _db.EmpleadosCredenciales
                .Update(
                    credencial);


            await _db
                .SaveChangesAsync();
        }


        // =====================================================
        // REEMPLAZAR
        // =====================================================

        public async Task ReemplazarAsync(
            EmpleadoCredencial? anterior,
            EmpleadoCredencial nueva)
        {
            await using var transaccion =
                await _db.Database
                    .BeginTransactionAsync();


            try
            {
                if (anterior != null)
                {
                    anterior.Estado =
                        false;


                    _db.EmpleadosCredenciales
                        .Update(
                            anterior);
                }


                await _db
                    .EmpleadosCredenciales
                    .AddAsync(
                        nueva);


                await _db
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();
            }
            catch
            {
                await transaccion
                    .RollbackAsync();

                throw;
            }
        }
    }
}