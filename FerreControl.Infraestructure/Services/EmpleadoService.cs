using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.Interfaces;
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

        return empleados.Select(x => new EmpleadoDto
        {
            ID = x.ID,
            NegocioID = x.NegocioID,
            Codigo = x.Codigo,
            Nombres = x.Nombres,
            Apellidos = x.Apellidos,
            Telefono = x.Telefono,
            Puesto = x.Puesto,
            Estado = x.Estado

        }).ToList();
    }

    public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
    {
        var empleado =
            await _empleadoRepository.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            return null;
        }

        return new EmpleadoDto
        {
            ID = empleado.ID,
            NegocioID = empleado.NegocioID,
            Codigo = empleado.Codigo,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Telefono = empleado.Telefono,
            Puesto = empleado.Puesto,
            Estado = empleado.Estado
        };
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
            FechaCreacion = DateTime.Now
        };

        await _empleadoRepository.AgregarAsync(empleado);

        return new EmpleadoDto
        {
            ID = empleado.ID,
            NegocioID = empleado.NegocioID,
            Codigo = empleado.Codigo,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Telefono = empleado.Telefono,
            Puesto = empleado.Puesto,
            Estado = empleado.Estado
        };
    }
}
