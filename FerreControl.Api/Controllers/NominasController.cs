using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominasController : ControllerBase
    {
        private readonly INominaService _nominaService;


        public NominasController(
            INominaService nominaService)
        {
            _nominaService =
                nominaService;
        }


        [HttpGet("negocio/{negocioId:int}")]
        public async Task<ActionResult<List<NominaDto>>>
            ObtenerPorNegocio(int negocioId)
        {
            return Ok(
                await _nominaService
                    .ObtenerPorNegocioAsync(
                        negocioId));
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<NominaDto>>
            ObtenerPorId(int id)
        {
            var nomina =
                await _nominaService
                    .ObtenerPorIdAsync(id);

            if (nomina == null)
                return NotFound();


            return Ok(nomina);
        }


        [HttpPost]
        public async Task<ActionResult<NominaDto>>
            Crear(CrearNominaDto dto)
        {
            try
            {
                var nomina =
                    await _nominaService
                        .CrearAsync(dto);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = nomina.ID },
                    nomina);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id:int}/estado")]
        public async Task<ActionResult<NominaDto>>
            ActualizarEstado(
                int id,
                ActualizarEstadoNominaDto dto)
        {
            try
            {
                return Ok(
                    await _nominaService
                        .ActualizarEstadoAsync(
                            id,
                            dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}