

using FerreControl.Application.DTOs.Empleados;

namespace FerreControl.Application.Interfaces;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();

    Task<EmpleadoDto?> ObtenerPorIdAsync(int id);

    Task<EmpleadoDto> CrearEmpleado(CrearEmpleadoDto dto);
}
