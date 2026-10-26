using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Desktop.Services;
using System.Windows;

namespace FerreControl.Desktop.Views
{
    public partial class AdelantosEmpleadoFormWindow: Window
    {
        private readonly AdelantoApiService
            _adelantoApi;

        private readonly int
            _empleadoId;


        public AdelantosEmpleadoFormWindow(
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


            Loaded +=
                AdelantosEmpleadoWindow_Loaded;
        }


        private async void
            AdelantosEmpleadoWindow_Loaded(
                object sender,
                RoutedEventArgs e)
        {
            await CargarAsync();
        }


        private async Task CargarAsync()
        {
            try
            {
                var adelantos =
                    await _adelantoApi
                        .ObtenerPorEmpleadoAsync(
                            _empleadoId);


                dgAdelantos.ItemsSource =
                    adelantos
                        .OrderByDescending(x =>
                            x.FechaAdelanto)
                        .ToList();


                var activos =
                    adelantos.Where(x =>
                        !string.Equals(
                            x.Estado,
                            "ANULADO",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();


                txtTotalEntregado.Text =
                    $"Q {activos.Sum(x => x.Monto):N2}";


                txtTotalAplicado.Text =
                    $"Q {activos.Sum(x => x.TotalAplicado):N2}";


                txtSaldoPendiente.Text =
                    $"Q {activos.Sum(x => x.SaldoPendiente):N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cargar el historial de adelantos.\n\n{ex.Message}",
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