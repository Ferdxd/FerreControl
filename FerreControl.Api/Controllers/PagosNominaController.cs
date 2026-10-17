using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace FerreControl.Api.Controllers
{
    [ApiController]
    [Route("api/nomina-detalles/{nominaDetalleId:int}/pagos")]
    public class PagosNominaController : ControllerBase
    {
        private readonly IPagoNominaService
            _pagoService;


        public PagosNominaController(
            IPagoNominaService pagoService)
        {
            _pagoService =
                pagoService;
        }


        [HttpGet]
        public async Task<ActionResult<List<PagoNominaDto>>>
            ObtenerPorDetalle(
                int nominaDetalleId)
        {
            return Ok(
                await _pagoService
                    .ObtenerPorDetalleAsync(
                        nominaDetalleId));
        }


        [HttpPost]
        public async Task<ActionResult<PagoNominaDto>>
            Registrar(
                int nominaDetalleId,
                CrearPagoNominaDto dto)
        {
            try
            {
                return Ok(
                    await _pagoService
                        .RegistrarAsync(
                            nominaDetalleId,
                            dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{pagoId:int}/anular")]
        public async Task<ActionResult<PagoNominaDto>>
            Anular(
                int nominaDetalleId,
                int pagoId)
        {
            try
            {
                return Ok(
                    await _pagoService
                        .AnularAsync(pagoId));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}