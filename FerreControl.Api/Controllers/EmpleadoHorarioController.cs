using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FerreControl.Application.DTOs.EmpleadoHorario;
using FerreControl.Application.Interfaces.IService;


namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/empleados/{empleadoId:int}/horarios")]
    public class EmpleadoHorarioController : ControllerBase
    {
        private readonly IEmpleadoHorarioService
            _empleadoHorarioService;


        public EmpleadoHorarioController(
            IEmpleadoHorarioService empleadoHorarioService)
        {
            _empleadoHorarioService =
                empleadoHorarioService;
        }


        [HttpGet]
        public async Task<ActionResult<List<EmpleadoHorarioDto>>>ObtenerHistorial(int empleadoId)
        {
            try
            {
                var horarios =
                    await _empleadoHorarioService
                        .ObtenerHistorialAsync(
                            empleadoId);

                return Ok(horarios);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("actual",Name = "ObtenerHorarioActualEmpleado")]
        public async Task<ActionResult<EmpleadoHorarioDto>>
            ObtenerActual(int empleadoId)
        {
            var horario =
                await _empleadoHorarioService
                    .ObtenerActualAsync(empleadoId);

            if (horario == null)
            {
                return NotFound(
                    "El empleado no tiene un horario asignado.");
            }

            return Ok(horario);
        }


        [HttpPost]
        public async Task<ActionResult<EmpleadoHorarioDto>>
            AsignarHorario(
                int empleadoId,
                CrearEmpleadoHorarioDto dto)
        {
            try
            {
                var horario =
                    await _empleadoHorarioService
                        .AsignarHorarioAsync(
                            empleadoId,
                            dto);

                return CreatedAtRoute(
                    "ObtenerHorarioActualEmpleado",
                    new
                    {
                        empleadoId =
                            horario.EmpleadoID
                    },
                    horario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
