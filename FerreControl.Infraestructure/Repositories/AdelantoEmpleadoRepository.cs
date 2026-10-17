using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Constants;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Repositories;

public class AdelantoEmpleadoRepository
    : IAdelantoEmpleadoRepository
{
    private readonly FerreControlDbContext _context;


    public AdelantoEmpleadoRepository(
        FerreControlDbContext context)
    {
        _context = context;
    }


    public async Task<List<AdelantoEmpleado>>
        ObtenerPorEmpleadoAsync(
            int empleadoId)
    {
        return await _context.AdelantosEmpleado
            .Where(x =>
                x.EmpleadoID == empleadoId)
            .OrderByDescending(x =>
                x.FechaAdelanto)
            .ThenByDescending(x =>
                x.ID)
            .ToListAsync();
    }


    public async Task<List<AdelantoEmpleado>>
        ObtenerPendientesPorEmpleadoAsync(
            int empleadoId)
    {
        return await _context.AdelantosEmpleado
            .Where(x =>
                x.EmpleadoID == empleadoId
                &&
                x.Estado != EstadosAdelanto.LIQUIDADO
                &&
                x.Estado != EstadosAdelanto.ANULADO)
            .OrderBy(x =>
                x.FechaAdelanto)
            .ToListAsync();
    }


    public async Task<AdelantoEmpleado?>
        ObtenerPorIdAsync(
            int id)
    {
        return await _context.AdelantosEmpleado
            .FirstOrDefaultAsync(x =>
                x.ID == id);
    }


    public async Task AgregarAsync(
        AdelantoEmpleado adelanto)
    {
        await _context.AdelantosEmpleado
            .AddAsync(adelanto);

        await _context.SaveChangesAsync();
    }


    public async Task ActualizarAsync(
        AdelantoEmpleado adelanto)
    {
        _context.AdelantosEmpleado
            .Update(adelanto);

        await _context.SaveChangesAsync();
    }
}