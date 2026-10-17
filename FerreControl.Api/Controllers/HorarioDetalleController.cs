using FerreControl.Application.DTOs.HorarioDetalle;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioDetalleController : ControllerBase
    {
        private readonly IHorarioDetalleService _horarioDetalleService;

        public HorarioDetalleController(
            IHorarioDetalleService horarioDetalleService)
        {
            _horarioDetalleService = horarioDetalleService;
        }


        [HttpGet("horario/{horarioId:int}")]
        public async Task<ActionResult<IEnumerable<HorarioDetalleDto>>>ObtenerPorHorarioAsync(int horarioId)
        {
            try
            {
                var horarioDetalles =await _horarioDetalleService.ObtenerPorHorarioAsync(horarioId);

                return Ok(horarioDetalles);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id:int}", Name = "ObtenerHorarioDetallePorId")]
        public async Task<ActionResult<HorarioDetalleDto?>>ObtenerPorIdAsync(int id)
        {
            var horarioDetalle =
                await _horarioDetalleService.ObtenerPorIdAsync(id);

            if (horarioDetalle == null)
            {
                return NotFound();
            }

            return Ok(horarioDetalle);
        }


        [HttpPost("horario/{horarioId:int}")]
        public async Task<ActionResult<HorarioDetalleDto>>
            CrearAsync(int horarioId,CrearHorarioDetalleDto dto)
        {
            try
            {
                var detalleHorario =
                    await _horarioDetalleService
                        .CrearAsync(horarioId, dto);

                return CreatedAtRoute(
                 "ObtenerHorarioDetallePorId",
                 new { id = detalleHorario.ID },
                 detalleHorario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<HorarioDetalleDto?>>
            ActualizarAsync(
                int id,
                ActualizarHorarioDetalleDto dto)
        {
            try
            {
                var detalleHorario =
                    await _horarioDetalleService
                        .ActualizarAsync(id, dto);

                if (detalleHorario == null)
                {
                    return NotFound();
                }

                return Ok(detalleHorario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}