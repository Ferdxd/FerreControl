using FerreControl.Application.DTOs.Nomina;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class PagoNominaFormWindow
        : Window
    {
        private readonly PagoNominaApiService
            _pagoApi;

        private readonly NominaDetalleFila
            _detalle;


        public PagoNominaFormWindow(
            NominaDetalleFila detalle)
        {
            InitializeComponent();


            _detalle =
                detalle;


            _pagoApi =
                new PagoNominaApiService();


            txtEmpleado.Text =
                detalle.Empleado;


            txtTotal.Text =
                $"Q {detalle.TotalPagar:N2}";


            txtPagado.Text =
                $"Q {detalle.TotalPagado:N2}";


            txtPendiente.Text =
                $"Q {detalle.Pendiente:N2}";


            txtMonto.Text =
                detalle.Pendiente
                    .ToString("0.00");


            dpFechaPago.SelectedDate =
                DateTime.Today;


            cmbMetodo.SelectedIndex =
                0;
        }


        private async void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!decimal.TryParse(
                txtMonto.Text,
                out decimal monto)
                ||
                monto <= 0)
            {
                MessageBox.Show(
                    "Ingrese un monto válido.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (monto >
                _detalle.Pendiente)
            {
                MessageBox.Show(
                    "El monto no puede ser mayor al saldo pendiente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!dpFechaPago.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha del pago.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (cmbMetodo.SelectedItem
                is not ComboBoxItem metodoItem)
            {
                MessageBox.Show(
                    "Seleccione el método de pago.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                btnGuardar.IsEnabled =
                    false;


                var dto =
                    new CrearPagoNominaDto
                    {
                        Monto =
                            monto,

                        FechaPago =
                            dpFechaPago
                                .SelectedDate
                                .Value.Date,

                        MetodoPago =
                            metodoItem.Content
                                .ToString()
                            ??
                            string.Empty,

                        Referencia =
                            string.IsNullOrWhiteSpace(
                                txtReferencia.Text)
                                ?
                                null
                                :
                                txtReferencia.Text.Trim(),

                        Observacion =
                            string.IsNullOrWhiteSpace(
                                txtObservacion.Text)
                                ?
                                null
                                :
                                txtObservacion.Text.Trim()
                    };


                await _pagoApi
                    .RegistrarAsync(
                        _detalle.ID,
                        dto);


                MessageBox.Show(
                    "El pago fue registrado correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo registrar el pago.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnGuardar.IsEnabled =
                    true;
            }
        }


        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult =
                false;
        }
    }
}