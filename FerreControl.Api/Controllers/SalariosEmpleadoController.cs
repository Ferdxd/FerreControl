using FerreControl.Application.DTOs.Salarios;
using FerreControl.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/empleados/{idEmpleado:int}/salarios")]
    public class SalariosEmpleadoController : ControllerBase
    {
        private readonly ISalarioEmpleadoService _salarioEmpleadoService;

        public SalariosEmpleadoController(
            ISalarioEmpleadoService salarioEmpleadoService)
        {
            _salarioEmpleadoService = salarioEmpleadoService;
        }


        [HttpGet]
        public async Task<ActionResult<List<SalarioEmpleadoDto>>>
            ObtenerHistorial(int idEmpleado)
        {
            try
            {
                var salarios =
                    await _salarioEmpleadoService
                        .ObtenerHistorialAsync(idEmpleado);

                return Ok(salarios);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("actual")]
        public async Task<ActionResult<SalarioEmpleadoDto>>
            ObtenerActual(int idEmpleado)
        {
            var salario =
                await _salarioEmpleadoService
                    .ObtenerActualAsync(idEmpleado);

            if (salario == null)
            {
                return NotFound(
                    "El empleado no tiene un salario actual.");
            }

            return Ok(salario);
        }


        [HttpPost]
        public async Task<ActionResult<SalarioEmpleadoDto>>
            RegistrarSalario(
                int idEmpleado,
                CrearSalarioEmpleadoDto dto)
        {
            try
            {
                var salario =
                    await _salarioEmpleadoService
                        .RegistrarSalarioAsync(
                            idEmpleado,
                            dto);

                return CreatedAtAction(
                    nameof(ObtenerActual),
                    new
                    {
                        idEmpleado = salario.EmpleadoID
                    },
                    salario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}