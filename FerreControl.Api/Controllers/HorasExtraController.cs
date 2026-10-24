using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers;

[ApiController]
[Route("api/horas-extra")]
public class HorasExtraController : ControllerBase
{
    private readonly IHoraExtraService _service;

    public HorasExtraController(IHoraExtraService service)
    {
        _service = service;
    }

    [HttpGet("reglas/negocio/{negocioId:int}")]
    public async Task<ActionResult<List<ReglaHoraExtraDto>>> ObtenerReglas(int negocioId)
    {
        return Ok(await _service.ObtenerReglasAsync(negocioId));
    }

    [HttpPost("reglas")]
    public async Task<ActionResult<ReglaHoraExtraDto>> CrearRegla(
        CrearReglaHoraExtraDto dto)
    {
        try
        {
            return StatusCode(
                StatusCodes.Status201Created,
                await _service.CrearReglaAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("empleados/{empleadoId:int}/actual")]
    public async Task<ActionResult<EmpleadoReglaHoraExtraDto>> ObtenerActual(
        int empleadoId)
    {
        var resultado =
            await _service.ObtenerConfiguracionActualAsync(empleadoId);

        if (resultado == null)
        {
            return NotFound(
                "El empleado no tiene una regla de horas extra asignada.");
        }

        return Ok(resultado);
    }

    [HttpPost("empleados/{empleadoId:int}/asignar")]
    public async Task<ActionResult<EmpleadoReglaHoraExtraDto>> Asignar(
        int empleadoId,
        AsignarReglaHoraExtraDto dto)
    {
        try
        {
            return StatusCode(
                StatusCodes.Status201Created,
                await _service.AsignarReglaAsync(empleadoId, dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("empleados/{empleadoId:int}/desactivar")]
    public async Task<ActionResult<EmpleadoReglaHoraExtraDto>> Desactivar(
        int empleadoId,
        DesactivarHoraExtraEmpleadoDto dto)
    {
        try
        {
            return Ok(
                await _service.DesactivarConfiguracionAsync(
                    empleadoId,
                    dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("calcular")]
    public async Task<ActionResult<List<RegistroHoraExtraDto>>> Calcular(
        CalcularHorasExtraDto dto)
    {
        try
        {
            return Ok(await _service.CalcularPeriodoAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("registros")]
    public async Task<ActionResult<List<RegistroHoraExtraDto>>> ObtenerRegistros(
        [FromQuery] int negocioId,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta)
    {
        try
        {
            return Ok(
                await _service.ObtenerRegistrosAsync(
                    negocioId,
                    desde,
                    hasta));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("registros/{registroId:int}/aprobar")]
    public async Task<ActionResult<RegistroHoraExtraDto>> Aprobar(
        int registroId,
        AprobarHoraExtraDto dto)
    {
        try
        {
            return Ok(await _service.AprobarAsync(registroId, dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("registros/{registroId:int}/rechazar")]
    public async Task<ActionResult<RegistroHoraExtraDto>> Rechazar(
        int registroId,
        RechazarHoraExtraDto dto)
    {
        try
        {
            return Ok(await _service.RechazarAsync(registroId, dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("nominas/{nominaId:int}/recalcular")]
    public async Task<ActionResult<ResultadoRecalculoNominaHoraExtraDto>>
        RecalcularNomina(int nominaId)
    {
        try
        {
            return Ok(await _service.RecalcularNominaAsync(nominaId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
