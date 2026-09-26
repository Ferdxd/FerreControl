using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpleadoDto>>> ObtenerTodos()
    {
        var empleados =
            await _empleadoService.ObtenerTodosAsync();

        return Ok(empleados);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmpleadoDto>> ObtenerPorId(int id)
    {
        var empleado =
            await _empleadoService.ObtenerPorIdAsync(id);

        if (empleado == null)
        {
            return NotFound();
        }

        return Ok(empleado);
    }

    [HttpPost]
    public async Task<ActionResult<EmpleadoDto>> Crear(
    CrearEmpleadoDto dto)
    {
        try
        {
            var empleado =
                await _empleadoService.CrearEmpleado(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = empleado.ID },
                empleado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}