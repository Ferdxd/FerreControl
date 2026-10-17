using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class AdelantoFormWindow
        : Window
    {
        private readonly AdelantoApiService
            _adelantoApi;

        private readonly int
            _empleadoId;


        public AdelantoFormWindow(
            int empleadoId,
            string nombreEmpleado)
        {
            InitializeComponent();


            _empleadoId =
                empleadoId;


            _adelantoApi =
                new AdelantoApiService();


            txtEmpleado.Text =
                nombreEmpleado;


            dpFecha.SelectedDate =
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


            if (!dpFecha.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha del adelanto.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (cmbMetodo.SelectedItem
                is not ComboBoxItem metodoItem)
            {
                MessageBox.Show(
                    "Seleccione el método de entrega.",
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
                    new CrearAdelantoEmpleadoDto
                    {
                        Monto =
                            monto,

                        FechaAdelanto =
                            dpFecha
                                .SelectedDate
                                .Value.Date,

                        MetodoEntrega =
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


                await _adelantoApi
                    .CrearAsync(
                        _empleadoId,
                        dto);


                MessageBox.Show(
                    "El adelanto fue registrado correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo registrar el adelanto.\n\n{ex.Message}",
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