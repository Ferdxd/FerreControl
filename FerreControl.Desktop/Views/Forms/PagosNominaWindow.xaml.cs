using FerreControl.Application.DTOs.Nomina;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class PagosNominaWindow
        : Window
    {
        private readonly PagoNominaApiService
            _pagoApi;

        private readonly NominaDetalleFila
            _detalle;


        private List<PagoNominaDto>
            _pagos;


        public bool HuboCambios { get; private set; }


        public PagosNominaWindow(
            NominaDetalleFila detalle)
        {
            InitializeComponent();


            _detalle =
                detalle;


            _pagoApi =
                new PagoNominaApiService();


            _pagos =
                new List<PagoNominaDto>();


            txtEmpleado.Text =
                detalle.Empleado;


            Loaded +=
                PagosNominaWindow_Loaded;
        }


        private async void
            PagosNominaWindow_Loaded(
                object sender,
                RoutedEventArgs e)
        {
            await CargarPagosAsync();
        }


        private async Task CargarPagosAsync()
        {
            try
            {
                _pagos =
                    await _pagoApi
                        .ObtenerPorDetalleAsync(
                            _detalle.ID);


                dgPagos.ItemsSource =
                    _pagos;


                var total =
                    _pagos
                        .Where(x =>
                            x.Estado)
                        .Sum(x =>
                            x.Monto);


                txtTotalPagado.Text =
                    $"Q {total:N2}";


                btnAnular.IsEnabled =
                    false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los pagos.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void DgPagos_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            btnAnular.IsEnabled =
                dgPagos.SelectedItem
                is PagoNominaDto pago
                &&
                pago.Estado;
        }


        private async void BtnAnular_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgPagos.SelectedItem
                is not PagoNominaDto pago)
            {
                return;
            }


            if (!pago.Estado)
                return;


            var confirmar =
                MessageBox.Show(
                    $"¿Desea anular el pago de Q {pago.Monto:N2}?",
                    "Anular pago",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                btnAnular.IsEnabled =
                    false;


                await _pagoApi
                    .AnularAsync(
                        _detalle.ID,
                        pago.ID);


                HuboCambios =
                    true;


                await CargarPagosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo anular el pago.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void BtnCerrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}