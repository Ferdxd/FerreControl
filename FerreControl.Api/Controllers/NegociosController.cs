using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.Negocios;
using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class NegociosController : ControllerBase
    {
        private readonly INegocioService _negocioService;

        public NegociosController(INegocioService negocioService)
        {
            _negocioService = negocioService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NegocioDto>>> ObtenerTodos()
        {
            var negocio = await _negocioService.ObtenerTodosAsync();
            return Ok(negocio);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NegocioDto>> ObtenerPorId(int id)
        {
            var negocio = await _negocioService.ObtenerPorIdAsync(id);
            if (negocio == null)
            {
                return NotFound();
            }
            return Ok(negocio);
        }
        [HttpPost]
        public async Task<ActionResult<NegocioDto>>Crear(CrearNegocioDto dto)
        {
            try
            {
                var negocio = await _negocioService.CrearNegocio(dto);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = negocio.ID },
                    negocio);

            }catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<NegocioDto>>Actualizar(int id,ActualizarNegocioDto dto)
        {
            try
            {
                var negocio = await _negocioService.ActualizarNegocio(id,dto);
                if (negocio == null)
                {
                    return NotFound();
                }

                return Ok(negocio);
            }catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
      

        }

    }
}
