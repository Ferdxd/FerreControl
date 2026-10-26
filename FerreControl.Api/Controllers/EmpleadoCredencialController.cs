using FerreControl.Application.DTOs.EmpleadoCredencial;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route(
        "api/empleados/{empleadoId:int}/credenciales")]
    public class EmpleadoCredencialController
        : ControllerBase
    {
        private readonly IEmpleadoCredencialService
            _service;


        public EmpleadoCredencialController(
            IEmpleadoCredencialService service)
        {
            _service =
                service;
        }


        // =====================================================
        // LISTAR
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<
            List<EmpleadoCredencialDto>>>
            Obtener(
                int empleadoId)
        {
            try
            {
                return Ok(
                    await _service
                        .ObtenerPorEmpleadoAsync(
                            empleadoId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // CREAR MANUAL
        // =====================================================

        [HttpPost]
        public async Task<ActionResult<
            EmpleadoCredencialDto>>
            Crear(
                int empleadoId,
                CrearEmpleadoCredencialDto dto)
        {
            try
            {
                var resultado =
                    await _service
                        .CrearAsync(
                            empleadoId,
                            dto);


                return StatusCode(
                    StatusCodes
                        .Status201Created,
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }


        // =====================================================
        // ESTADO
        // =====================================================

        [HttpPut("{credencialId:int}/estado")]
        public async Task<ActionResult<
            EmpleadoCredencialDto>>
            ActualizarEstado(
                int empleadoId,
                int credencialId,
                ActualizarEstadoCredencialDto dto)
        {
            try
            {
                var resultado =
                    await _service
                        .ActualizarEstadoAsync(
                            credencialId,
                            dto.Estado);


                if (resultado == null)
                    return NotFound();


                if (resultado.EmpleadoID !=
                    empleadoId)
                {
                    return BadRequest(
                        "La credencial no pertenece al empleado indicado.");
                }


                return Ok(
                    resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }
    }
}