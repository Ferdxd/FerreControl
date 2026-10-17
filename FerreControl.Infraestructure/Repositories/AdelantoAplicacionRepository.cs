using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Repositories;

public class AdelantoAplicacionRepository
    : IAdelantoAplicacionRepository
{
    private readonly FerreControlDbContext _context;


    public AdelantoAplicacionRepository(
        FerreControlDbContext context)
    {
        _context = context;
    }


    public async Task<List<AdelantoAplicacion>>
        ObtenerPorAdelantoAsync(
            int adelantoId)
    {
        return await _context.AdelantosAplicaciones
            .Where(x =>
                x.AdelantoEmpleadoID == adelantoId)
            .OrderByDescending(x =>
                x.FechaAplicacion)
            .ToListAsync();
    }


    public async Task<List<AdelantoAplicacion>>
        ObtenerPorNominaDetalleAsync(
            int nominaDetalleId)
    {
        return await _context.AdelantosAplicaciones
            .Where(x =>
                x.NominaDetalleID == nominaDetalleId)
            .OrderByDescending(x =>
                x.FechaAplicacion)
            .ToListAsync();
    }


    public async Task<AdelantoAplicacion?>
        ObtenerPorIdAsync(
            int id)
    {
        return await _context.AdelantosAplicaciones
            .FirstOrDefaultAsync(x =>
                x.ID == id);
    }


    public async Task<AdelantoAplicacion?>
        ObtenerActivaAsync(
            int adelantoId,
            int nominaDetalleId)
    {
        return await _context.AdelantosAplicaciones
            .FirstOrDefaultAsync(x =>
                x.AdelantoEmpleadoID == adelantoId
                &&
                x.NominaDetalleID == nominaDetalleId
                &&
                x.Estado);
    }


    public async Task<long>
        ObtenerTotalAplicadoCentavosAsync(
            int adelantoId)
    {
        return await _context.AdelantosAplicaciones
            .Where(x =>
                x.AdelantoEmpleadoID == adelantoId
                &&
                x.Estado)
            .Select(x =>
                (long?)x.MontoCentavos)
            .SumAsync()
            ?? 0;
    }


    public async Task<long>
        ObtenerTotalAplicadoNominaCentavosAsync(
            int nominaDetalleId)
    {
        return await _context.AdelantosAplicaciones
            .Where(x =>
                x.NominaDetalleID == nominaDetalleId
                &&
                x.Estado)
            .Select(x =>
                (long?)x.MontoCentavos)
            .SumAsync()
            ?? 0;
    }


    public async Task<Dictionary<int, long>>
        ObtenerTotalesPorAdelantosAsync(
            IEnumerable<int> adelantoIds)
    {
        var ids = adelantoIds
            .Distinct()
            .ToList();


        if (!ids.Any())
            return new Dictionary<int, long>();


        return await _context.AdelantosAplicaciones
            .Where(x =>
                ids.Contains(
                    x.AdelantoEmpleadoID)
                &&
                x.Estado)
            .GroupBy(x =>
                x.AdelantoEmpleadoID)
            .Select(x => new
            {
                AdelantoID = x.Key,
                Total = x.Sum(y =>
                    y.MontoCentavos)
            })
            .ToDictionaryAsync(
                x => x.AdelantoID,
                x => x.Total);
    }


    public async Task AgregarAsync(
        AdelantoAplicacion aplicacion)
    {
        await _context.AdelantosAplicaciones
            .AddAsync(aplicacion);

        await _context.SaveChangesAsync();
    }


    public async Task ActualizarAsync(
        AdelantoAplicacion aplicacion)
    {
        _context.AdelantosAplicaciones
            .Update(aplicacion);

        await _context.SaveChangesAsync();
    }
}