using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infraestructure.Repositories;

public class ReglaHoraExtraRepository
    : IReglaHoraExtraRepository
{
    private readonly FerreControlDbContext _db;

    public ReglaHoraExtraRepository(
        FerreControlDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReglaHoraExtra>>
        ObtenerPorNegocioAsync(int negocioId)
    {
        return await _db.ReglasHoraExtra
            .AsNoTracking()
            .Where(x => x.NegocioID == negocioId)
            .OrderByDescending(x => x.Estado)
            .ThenBy(x => x.Nombre)
            .ToListAsync();
    }

    public async Task<ReglaHoraExtra?> ObtenerPorIdAsync(
        int id)
    {
        return await _db.ReglasHoraExtra
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task AgregarAsync(
        ReglaHoraExtra regla)
    {
        await _db.ReglasHoraExtra.AddAsync(regla);
        await _db.SaveChangesAsync();
    }
}
