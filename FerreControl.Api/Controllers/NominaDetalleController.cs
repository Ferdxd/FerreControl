using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/nominas/{nominaId:int}/detalles")]
    public class NominaDetalleController : ControllerBase
    {
        private readonly INominaDetalleService
            _detalleService;


        public NominaDetalleController(
            INominaDetalleService detalleService)
        {
            _detalleService =
                detalleService;
        }


        [HttpGet]
        public async Task<ActionResult<List<NominaDetalleDto>>>
            ObtenerPorNomina(int nominaId)
        {
            return Ok(
                await _detalleService
                    .ObtenerPorNominaAsync(
                        nominaId));
        }


        [HttpPost]
        public async Task<ActionResult<NominaDetalleDto>>
            Crear(
                int nominaId,
                CrearNominaDetalleDto dto)
        {
            try
            {
                var detalle =
                    await _detalleService
                        .CrearAsync(
                            nominaId,
                            dto);

                return Ok(detalle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}