using FerreControl.Application.DTOs.EmpleadoCredencial;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/carnes")]
    public class CarnesController
        : ControllerBase
    {
        private readonly IEmpleadoCredencialService
            _service;


        public CarnesController(
            IEmpleadoCredencialService service)
        {
            _service =
                service;
        }


        // =====================================================
        // EMPLEADOS ACTIVOS + CARNÉ
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<
            List<CarnetEmpleadoDto>>>
            Obtener(
                [FromQuery] int negocioId)
        {
            return Ok(
                await _service
                    .ObtenerCarnesActivosAsync(
                        negocioId));
        }


        // =====================================================
        // GENERAR
        // =====================================================

        [HttpPost(
            "empleados/{empleadoId:int}/generar")]
        public async Task<ActionResult<
            EmpleadoCredencialDto>>
            Generar(
                int empleadoId)
        {
            try
            {
                return Ok(
                    await _service
                        .GenerarCodigoBarrasAsync(
                            empleadoId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // REEMPLAZAR
        // =====================================================

        [HttpPost(
            "empleados/{empleadoId:int}/reemplazar")]
        public async Task<ActionResult<
            EmpleadoCredencialDto>>
            Reemplazar(
                int empleadoId)
        {
            try
            {
                return Ok(
                    await _service
                        .ReemplazarCodigoBarrasAsync(
                            empleadoId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        [HttpPut(
            "empleados/{empleadoId:int}/desactivar")]
        public async Task<ActionResult<
            EmpleadoCredencialDto>>
            Desactivar(
                int empleadoId)
        {
            var resultado =
                await _service
                    .DesactivarCodigoBarrasAsync(
                        empleadoId);


            if (resultado == null)
            {
                return NotFound(
                    "El empleado no posee un código de barras activo.");
            }


            return Ok(
                resultado);
        }


        // =====================================================
        // GENERAR TODOS LOS FALTANTES
        // =====================================================

        [HttpPost("generar-faltantes")]
        public async Task<ActionResult<
            List<EmpleadoCredencialDto>>>
            GenerarFaltantes(
                [FromQuery] int negocioId)
        {
            try
            {
                return Ok(
                    await _service
                        .GenerarCodigosFaltantesAsync(
                            negocioId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }
    }
}