using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories
{
    public class MarcacionAsistenciaRepository
        : IMarcacionAsistenciaRepository
    {
        private readonly FerreControlDbContext _db;

        public MarcacionAsistenciaRepository(
            FerreControlDbContext db)
        {
            _db = db;
        }


        public async Task<List<MarcacionAsistencia>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha)
        {
            var inicio = fecha.Date;

            var fin =
                inicio.AddDays(1);


            return await _db.MarcacionesAsistencia
                .AsNoTracking()
                .Where(x =>
                    x.EmpleadoID == empleadoId &&
                    x.FechaHora >= inicio &&
                    x.FechaHora < fin)
                .OrderBy(x => x.FechaHora)
                .ToListAsync();
        }


        public async Task<bool>
            ExisteTipoEnFechaAsync(
                int empleadoId,
                string tipoMarcacion,
                DateTime fecha)
        {
            var inicio = fecha.Date;

            var fin =
                inicio.AddDays(1);


            return await _db.MarcacionesAsistencia
                .AnyAsync(x =>
                    x.EmpleadoID == empleadoId &&
                    x.TipoMarcacion ==
                        tipoMarcacion &&
                    x.FechaHora >= inicio &&
                    x.FechaHora < fin);
        }


        public async Task AgregarAsync(
            MarcacionAsistencia marcacion)
        {
            await _db.MarcacionesAsistencia
                .AddAsync(marcacion);

            await _db.SaveChangesAsync();
        }
    }
}
