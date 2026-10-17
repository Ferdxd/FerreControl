using FerreControl.Application.DTOs.Horarios;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorariosController : ControllerBase
    {
        private readonly IHorarioService _horarioService;
        
        public HorariosController(IHorarioService horarioService)
        {
            _horarioService = horarioService;
        }
        [HttpGet("negocio/{negocioId:int}")]
        public async Task<ActionResult<IEnumerable<HorarioDto>>> ObtenerHorarioPorNegocio(int negocioId)
        {
            var horarios = await _horarioService.ObtenerHorariosPorNegocioIdAsync(negocioId);
            return Ok(horarios);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<HorarioDto>> ObtenerHorarioPorId(int id)
        {
            var horario = await _horarioService.ObtenerHorarioPorIdAsync(id);
            if (horario == null)
            {
                return NotFound();
            }
            return Ok(horario);
        }

        [HttpPost]
        public async Task<ActionResult<HorarioDto>> CrearHorario(CrearHorarioDto dto)
        {
            try
            {
                var horario = await _horarioService.CrearHorarioAsync(dto);
                return CreatedAtAction(
                    nameof(ObtenerHorarioPorId),
                    new { id = horario.ID },
                    horario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<HorarioDto>> ActualizarHorario(int id, ActualizarHorarioDto dto)
        {
            try
            {
                var horario = await _horarioService.ActualizarHorarioAsync(id, dto);
                return Ok(horario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
