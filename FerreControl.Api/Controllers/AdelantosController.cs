using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers;

[ApiController]
public class AdelantosController : ControllerBase
{
    private readonly IAdelantoEmpleadoService
        _adelantoService;


    public AdelantosController(
        IAdelantoEmpleadoService adelantoService)
    {
        _adelantoService =
            adelantoService;
    }


    [HttpGet(
        "api/empleados/{empleadoId:int}/adelantos")]
    public async Task<IActionResult>
        ObtenerPorEmpleado(
            int empleadoId)
    {
        var resultado =
            await _adelantoService
                .ObtenerPorEmpleadoAsync(
                    empleadoId);

        return Ok(resultado);
    }


    [HttpGet(
        "api/empleados/{empleadoId:int}/adelantos/pendientes")]
    public async Task<IActionResult>
        ObtenerPendientes(
            int empleadoId)
    {
        var resultado =
            await _adelantoService
                .ObtenerPendientesPorEmpleadoAsync(
                    empleadoId);

        return Ok(resultado);
    }


    [HttpGet(
        "api/adelantos/{id:int}")]
    public async Task<IActionResult>
        ObtenerPorId(
            int id)
    {
        var resultado =
            await _adelantoService
                .ObtenerPorIdAsync(
                    id);


        if (resultado == null)
            return NotFound();


        return Ok(resultado);
    }


    [HttpPost(
        "api/empleados/{empleadoId:int}/adelantos")]
    public async Task<IActionResult>
        Crear(
            int empleadoId,
            CrearAdelantoEmpleadoDto dto)
    {
        try
        {
            var resultado =
                await _adelantoService
                    .CrearAsync(
                        empleadoId,
                        dto);

            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    mensaje = ex.Message
                });
        }
    }


    [HttpPut(
        "api/adelantos/{id:int}/anular")]
    public async Task<IActionResult>
        Anular(
            int id,
            AnularAdelantoEmpleadoDto dto)
    {
        try
        {
            var resultado =
                await _adelantoService
                    .AnularAsync(
                        id,
                        dto);

            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    mensaje = ex.Message
                });
        }
    }
}