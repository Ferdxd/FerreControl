using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers;

[ApiController]
public class AdelantoAplicacionesController
    : ControllerBase
{
    private readonly IAdelantoAplicacionService
        _aplicacionService;


    public AdelantoAplicacionesController(
        IAdelantoAplicacionService aplicacionService)
    {
        _aplicacionService =
            aplicacionService;
    }


    [HttpGet(
        "api/adelantos/{adelantoId:int}/aplicaciones")]
    public async Task<IActionResult>
        ObtenerPorAdelanto(
            int adelantoId)
    {
        var resultado =
            await _aplicacionService
                .ObtenerPorAdelantoAsync(
                    adelantoId);

        return Ok(resultado);
    }


    [HttpGet(
        "api/nomina-detalles/{nominaDetalleId:int}/adelantos")]
    public async Task<IActionResult>
        ObtenerPorNomina(
            int nominaDetalleId)
    {
        var resultado =
            await _aplicacionService
                .ObtenerPorNominaDetalleAsync(
                    nominaDetalleId);

        return Ok(resultado);
    }


    [HttpPost(
        "api/adelantos/{adelantoId:int}/aplicaciones")]
    public async Task<IActionResult>
        Aplicar(
            int adelantoId,
            CrearAdelantoAplicacionDto dto)
    {
        try
        {
            var resultado =
                await _aplicacionService
                    .AplicarAsync(
                        adelantoId,
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
        "api/adelanto-aplicaciones/{id:int}/anular")]
    public async Task<IActionResult>
        Anular(
            int id,
            AnularAdelantoAplicacionDto dto)
    {
        try
        {
            var resultado =
                await _aplicacionService
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