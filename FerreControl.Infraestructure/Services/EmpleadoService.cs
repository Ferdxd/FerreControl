using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadoService(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
    {
        var empleados =
            await _empleadoRepository.ObtenerTodosAsync();

        return empleados.Select(MapearDto).ToList();
    }

    public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
    {
        var empleado =
            await _empleadoRepository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            return null;
        }

        return MapearDto(empleado);
    }

    public async Task<EmpleadoDto> CrearEmpleado(CrearEmpleadoDto dto)
    {
        var codigoExiste =
            await _empleadoRepository.ExisteCodigoAsync(
                dto.NegocioID,
                dto.Codigo);

        if (codigoExiste)
        {
            throw new InvalidOperationException(
                "Ya existe un empleado con ese código en este negocio.");
        }

        var empleado = new Empleado
        {
            NegocioID = dto.NegocioID,
            Codigo = dto.Codigo.Trim(),
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Puesto = dto.Puesto?.Trim(),
            Estado = true,
            FechaIngreso=dto.FechaIngreso,
            FechaCreacion = DateTime.Now
        };

        await _empleadoRepository.AgregarAsync(empleado);

        return MapearDto(empleado);
    }

    public async Task<EmpleadoDto> ActualizarEmpleado(int id, ActualizarEmpleadoDto dto)
    {
        var empleado = await _empleadoRepository.ObtenerPorIdAsync(id);
        if(empleado == null)
        {
            throw new InvalidOperationException("Empleado no encontrado.");
        }
        if (empleado.Estado && !dto.Estado)
        {
            empleado.FechaBaja = DateTime.Now;
        }
        else if (!empleado.Estado && dto.Estado)
        {
            empleado.FechaBaja = null;
        }
        empleado.Codigo = dto.Codigo.Trim();
        empleado.Nombres = dto.Nombres.Trim();
        empleado.Apellidos = dto.Apellidos.Trim();
        empleado.Telefono = dto.Telefono?.Trim();
        empleado.Puesto = dto.Puesto?.Trim();
        empleado.Estado = dto.Estado;

        await _empleadoRepository.ActualizarAsync(empleado);
        return MapearDto(empleado);

    }

    private EmpleadoDto MapearDto(Empleado empleado)
    {
        return new EmpleadoDto
        {
            ID = empleado.ID,
            NegocioID = empleado.NegocioID,
            Codigo = empleado.Codigo,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Telefono = empleado.Telefono,
            Puesto = empleado.Puesto,
            Estado = empleado.Estado,
            FechaIngreso = empleado.FechaIngreso,
            FechaCreacion = empleado.FechaCreacion,
            FechaBaja = empleado.FechaBaja
        };
    }
}
