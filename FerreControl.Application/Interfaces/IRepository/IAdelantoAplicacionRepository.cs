using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository;

public interface IAdelantoAplicacionRepository
{
    Task<List<AdelantoAplicacion>> ObtenerPorAdelantoAsync(
        int adelantoId);

    Task<List<AdelantoAplicacion>> ObtenerPorNominaDetalleAsync(
        int nominaDetalleId);

    Task<AdelantoAplicacion?> ObtenerPorIdAsync(
        int id);

    Task<AdelantoAplicacion?> ObtenerActivaAsync(
        int adelantoId,
        int nominaDetalleId);

    Task<long> ObtenerTotalAplicadoCentavosAsync(
        int adelantoId);

    Task<long> ObtenerTotalAplicadoNominaCentavosAsync(
        int nominaDetalleId);

    Task<Dictionary<int, long>> ObtenerTotalesPorAdelantosAsync(
        IEnumerable<int> adelantoIds);

    Task AgregarAsync(
        AdelantoAplicacion aplicacion);

    Task ActualizarAsync(
        AdelantoAplicacion aplicacion);
}