using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IPagoNominaRepository
    {
        Task<List<PagoNomina>> ObtenerPorDetalleAsync(
            int nominaDetalleId);

        Task<PagoNomina?> ObtenerPorIdAsync(int id);

        Task<long> ObtenerTotalPagadoCentavosAsync(
            int nominaDetalleId);

        Task AgregarAsync(PagoNomina pago);

        Task ActualizarAsync(PagoNomina pago);
    }
}