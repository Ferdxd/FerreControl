using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories;

public class EmpleadoReglaHoraExtraRepository
    : IEmpleadoReglaHoraExtraRepository
{
    private readonly FerreControlDbContext _db;

    public EmpleadoReglaHoraExtraRepository(
        FerreControlDbContext db)
    {
        _db = db;
    }

    public async Task<List<EmpleadoReglaHoraExtra>>
        ObtenerHistorialAsync(int empleadoId)
    {
        return await _db.EmpleadosReglasHoraExtra
            .AsNoTracking()
            .Include(x => x.ReglaHoraExtra)
            .Where(x => x.EmpleadoID == empleadoId)
            .OrderByDescending(x => x.FechaInicio)
            .ToListAsync();
    }

    public async Task<EmpleadoReglaHoraExtra?>
        ObtenerActualAsync(int empleadoId)
    {
        return await _db.EmpleadosReglasHoraExtra
            .AsNoTracking()
            .Include(x => x.ReglaHoraExtra)
            .FirstOrDefaultAsync(x =>
                x.EmpleadoID == empleadoId &&
                x.Estado &&
                x.FechaFin == null);
    }

    public async Task<EmpleadoReglaHoraExtra?>
        ObtenerVigenteEnFechaAsync(
            int empleadoId,
            DateTime fecha)
    {
        var dia = fecha.Date;

        return await _db.EmpleadosReglasHoraExtra
            .AsNoTracking()
            .Include(x => x.ReglaHoraExtra)
            .FirstOrDefaultAsync(x =>
                x.EmpleadoID == empleadoId &&
                x.FechaInicio.Date <= dia &&
                (
                    x.FechaFin == null ||
                    x.FechaFin.Value.Date >= dia
                ));
    }

    public async Task AgregarAsync(
        EmpleadoReglaHoraExtra configuracion)
    {
        await _db.EmpleadosReglasHoraExtra
            .AddAsync(configuracion);

        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(
        EmpleadoReglaHoraExtra configuracion)
    {
        _db.EmpleadosReglasHoraExtra
            .Update(configuracion);

        await _db.SaveChangesAsync();
    }
}
