using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using FerreControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly FerreControlDbContext _context;

    public EmpleadoRepository(FerreControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Empleado>> ObtenerTodosAsync()
    {
        return await _context.Empleados
            .AsNoTracking()
            .OrderBy(x => x.Nombres)
            .ThenBy(x => x.Apellidos)
            .ToListAsync();
    }

    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        return await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<bool> ExisteCodigoAsync(
        int negocioID,
        string codigo,
        int? empleadoIDExcluir = null)
    {
        return await _context.Empleados.AnyAsync(x =>
            x.NegocioID == negocioID &&
            x.Codigo == codigo &&
            (
                empleadoIDExcluir == null ||
                x.ID != empleadoIDExcluir
            ));
    }

    public async Task AgregarAsync(Empleado empleado)
    {
        await _context.Empleados.AddAsync(empleado);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);

        await _context.SaveChangesAsync();
    }
}