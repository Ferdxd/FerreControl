using FerreControl.Application.DTOs.Adelantos;

namespace FerreControl.Application.Interfaces.IService;

public interface IAdelantoAplicacionService
{
    Task<List<AdelantoAplicacionDto>>
        ObtenerPorAdelantoAsync(
            int adelantoId);

    Task<List<AdelantoAplicacionDto>>
        ObtenerPorNominaDetalleAsync(
            int nominaDetalleId);

    Task<AdelantoAplicacionDto>
        AplicarAsync(
            int adelantoId,
            CrearAdelantoAplicacionDto dto);

    Task<AdelantoAplicacionDto>
        AnularAsync(
            int id,
            AnularAdelantoAplicacionDto dto);
}