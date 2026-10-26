using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories;

public class RegistroHoraExtraRepository
    : IRegistroHoraExtraRepository
{
    private readonly FerreControlDbContext _db;

    public RegistroHoraExtraRepository(
        FerreControlDbContext db)
    {
        _db = db;
    }

    public async Task<RegistroHoraExtra?> ObtenerPorIdAsync(
        int id)
    {
        return await _db.RegistrosHoraExtra
            .Include(x => x.Empleado)
            .Include(x => x.EmpleadoReglaHoraExtra)
                .ThenInclude(x => x!.ReglaHoraExtra)
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<RegistroHoraExtra?>
        ObtenerPorEmpleadoFechaAsync(
            int empleadoId,
            DateTime fecha)
    {
        var dia = fecha.Date;

        return await _db.RegistrosHoraExtra
            .Include(x => x.Empleado)
            .Include(x => x.EmpleadoReglaHoraExtra)
                .ThenInclude(x => x!.ReglaHoraExtra)
            .FirstOrDefaultAsync(x =>
                x.EmpleadoID == empleadoId &&
                x.Fecha.Date == dia);
    }

    public async Task<List<RegistroHoraExtra>>
        ObtenerPorNegocioPeriodoAsync(
            int negocioId,
            DateTime desde,
            DateTime hasta)
    {
        var inicio = desde.Date;
        var fin = hasta.Date;

        return await _db.RegistrosHoraExtra
            .AsNoTracking()
            .Include(x => x.Empleado)
            .Include(x => x.EmpleadoReglaHoraExtra)
                .ThenInclude(x => x!.ReglaHoraExtra)
            .Where(x =>
                x.Empleado.NegocioID == negocioId &&
                x.Fecha.Date >= inicio &&
                x.Fecha.Date <= fin)
            .OrderByDescending(x => x.Fecha)
            .ThenBy(x => x.Empleado.Nombres)
            .ThenBy(x => x.Empleado.Apellidos)
            .ToListAsync();
    }

    public async Task AgregarAsync(
        RegistroHoraExtra registro)
    {
        await _db.RegistrosHoraExtra
            .AddAsync(registro);

        await _db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(
        RegistroHoraExtra registro)
    {
        _db.RegistrosHoraExtra
            .Update(registro);

        await _db.SaveChangesAsync();
    }
}
