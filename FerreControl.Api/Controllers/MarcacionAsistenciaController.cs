using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Application.Interfaces.IService;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/marcaciones")]
    public class MarcacionesController
        : ControllerBase
    {
        private readonly IMarcacionAsistenciaService
            _service;


        public MarcacionesController(
            IMarcacionAsistenciaService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<ActionResult<
            MarcacionAsistenciaDto>>
            Registrar(
                RegistrarMarcacionDto dto)
        {
            try
            {
                var marcacion =
                    await _service
                        .RegistrarAsync(dto);

                return StatusCode(
                    StatusCodes.Status201Created,
                    marcacion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet(
            "empleado/{empleadoId:int}")]
        public async Task<ActionResult<
            List<MarcacionAsistenciaDto>>>
            Obtener(
                int empleadoId,
                [FromQuery] DateTime fecha)
        {
            var resultado =
                await _service
                    .ObtenerPorEmpleadoYFechaAsync(
                        empleadoId,
                        fecha);

            return Ok(resultado);
        }
    }
}