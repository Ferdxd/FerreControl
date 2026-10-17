using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class AplicarAdelantoWindow
        : Window
    {
        private readonly AdelantoApiService
            _adelantoApi;

        private readonly NominaDetalleFila
            _detalle;


        public AplicarAdelantoWindow(
            NominaDetalleFila detalle)
        {
            InitializeComponent();


            _detalle =
                detalle;


            _adelantoApi =
                new AdelantoApiService();


            txtEmpleado.Text =
                detalle.Empleado;


            Loaded +=
                AplicarAdelantoWindow_Loaded;
        }


        private async void
            AplicarAdelantoWindow_Loaded(
                object sender,
                RoutedEventArgs e)
        {
            await CargarAdelantosAsync();
        }


        private async Task CargarAdelantosAsync()
        {
            try
            {
                var adelantos =
                    await _adelantoApi
                        .ObtenerPendientesPorEmpleadoAsync(
                            _detalle.EmpleadoID);


                var opciones =
                    adelantos
                        .Where(x =>
                            x.SaldoPendiente > 0)
                        .Select(x =>
                            new AdelantoOpcion
                            {
                                Adelanto =
                                    x,

                                Descripcion =
                                    $"{x.FechaAdelanto:dd/MM/yyyy} - " +
                                    $"Saldo Q {x.SaldoPendiente:N2}"
                            })
                        .ToList();


                cmbAdelanto.ItemsSource =
                    opciones;


                if (opciones.Any())
                {
                    cmbAdelanto.SelectedIndex =
                        0;
                }
                else
                {
                    btnAplicar.IsEnabled =
                        false;


                    MessageBox.Show(
                        "El empleado no tiene adelantos pendientes.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los adelantos.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void CmbAdelanto_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbAdelanto.SelectedItem
                is not AdelantoOpcion opcion)
            {
                txtMontoOriginal.Text =
                    "Q 0.00";

                txtSaldo.Text =
                    "Q 0.00";

                txtMonto.Text =
                    string.Empty;

                return;
            }


            txtMontoOriginal.Text =
                $"Q {opcion.Adelanto.Monto:N2}";


            txtSaldo.Text =
                $"Q {opcion.Adelanto.SaldoPendiente:N2}";


            txtMonto.Text =
                opcion.Adelanto
                    .SaldoPendiente
                    .ToString("0.00");
        }


        private async void BtnAplicar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbAdelanto.SelectedItem
                is not AdelantoOpcion opcion)
            {
                MessageBox.Show(
                    "Seleccione un adelanto.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


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
                opcion.Adelanto.SaldoPendiente)
            {
                MessageBox.Show(
                    "El monto no puede ser mayor al saldo pendiente del adelanto.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                btnAplicar.IsEnabled =
                    false;


                var dto =
                    new CrearAdelantoAplicacionDto
                    {
                        NominaDetalleID =
                            _detalle.ID,

                        Monto =
                            monto,

                        Observacion =
                            string.IsNullOrWhiteSpace(
                                txtObservacion.Text)
                                ?
                                null
                                :
                                txtObservacion.Text.Trim()
                    };


                await _adelantoApi
                    .AplicarAsync(
                        opcion.Adelanto.ID,
                        dto);


                MessageBox.Show(
                    "El adelanto fue aplicado correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo aplicar el adelanto.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnAplicar.IsEnabled =
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


        private class AdelantoOpcion
        {
            public AdelantoEmpleadoDto Adelanto { get; set; }
                = null!;

            public string Descripcion { get; set; }
                = string.Empty;
        }
    }
}