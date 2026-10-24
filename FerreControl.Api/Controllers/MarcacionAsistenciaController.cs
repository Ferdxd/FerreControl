using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

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
            _service =
                service;
        }


        // =====================================================
        // MARCACIÓN EXPLÍCITA
        // =====================================================

        [HttpPost]
        public async Task<ActionResult<MarcacionAsistenciaDto>>
            Registrar(
                RegistrarMarcacionDto dto)
        {
            try
            {
                var resultado =
                    await _service
                        .RegistrarAsync(
                            dto);


                return StatusCode(
                    StatusCodes.Status201Created,
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // MARCAJE AUTOMÁTICO
        // =====================================================

        [HttpPost("automatico")]
        public async Task<ActionResult<MarcacionAsistenciaDto>>
            RegistrarAutomatico(
                RegistrarMarcacionAutomaticaDto dto)
        {
            try
            {
                var resultado =
                    await _service
                        .RegistrarAutomaticoAsync(
                            dto);


                return StatusCode(
                    StatusCodes.Status201Created,
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // REGISTRO MANUAL
        // =====================================================

        [HttpPost("manual")]
        public async Task<ActionResult<List<MarcacionAsistenciaDto>>>
            RegistrarManual(
                RegistrarAsistenciaManualDto dto)
        {
            try
            {
                var resultado =
                    await _service
                        .RegistrarManualAsync(
                            dto);


                return StatusCode(
                    StatusCodes.Status201Created,
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // MARCACIONES DEL EMPLEADO
        // =====================================================

        [HttpGet("empleado/{empleadoId:int}")]
        public async Task<ActionResult<List<MarcacionAsistenciaDto>>>
            Obtener(
                int empleadoId,
                [FromQuery] DateTime fecha)
        {
            try
            {
                var resultado =
                    await _service
                        .ObtenerPorEmpleadoYFechaAsync(
                            empleadoId,
                            fecha);


                return Ok(
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // RESUMEN DE ASISTENCIA
        // =====================================================

        [HttpGet("resumen")]
        public async Task<ActionResult<List<AsistenciaDiariaDto>>>
            ObtenerResumen(
                [FromQuery] int negocioId,
                [FromQuery] DateTime fecha)
        {
            var resultado =
                await _service
                    .ObtenerResumenDiarioAsync(
                        negocioId,
                        fecha);


            return Ok(
                resultado);
        }
    }
}