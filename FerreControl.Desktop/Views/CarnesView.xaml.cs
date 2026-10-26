using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class CarnesView
        : UserControl
    {
        private const int NegocioID =
            1;


        private readonly CarnetApiService
            _api;

        private readonly CodigoBarrasService
            _codigoBarras;

        private readonly CarnetPrintService
            _impresion;


        private List<CarnetEmpleadoFila>
            _filas;


        public CarnesView()
        {
            InitializeComponent();


            _api =
                new CarnetApiService();


            _codigoBarras =
                new CodigoBarrasService();


            _impresion =
                new CarnetPrintService(
                    _codigoBarras);


            _filas =
                new List<CarnetEmpleadoFila>();


            cmbEstado.SelectedIndex =
                0;


            Loaded +=
                CarnesView_Loaded;
        }


        // =====================================================
        // CARGAR
        // =====================================================

        private async void CarnesView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarAsync();
        }


        private async Task CargarAsync(
            int? empleadoSeleccionadoId = null)
        {
            try
            {
                var datos =
                    await _api
                        .ObtenerCarnesAsync(
                            NegocioID);


                _filas =
                    datos
                        .Select(
                            CarnetEmpleadoFila
                                .DesdeDto)
                        .ToList();


                ActualizarIndicadores();

                AplicarFiltros();


                if (empleadoSeleccionadoId
                    .HasValue)
                {
                    dgCarnes.SelectedItem =
                        dgCarnes.Items
                            .Cast<CarnetEmpleadoFila>()
                            .FirstOrDefault(x =>
                                x.EmpleadoID ==
                                empleadoSeleccionadoId.Value);
                }
                else
                {
                    LimpiarVistaPrevia();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los carnés.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // INDICADORES
        // =====================================================

        private void ActualizarIndicadores()
        {
            txtTotalActivos.Text =
                _filas.Count
                    .ToString();


            txtConCarnet.Text =
                _filas
                    .Count(x =>
                        x.TieneCodigoBarras)
                    .ToString();


            txtSinCarnet.Text =
                _filas
                    .Count(x =>
                        !x.TieneCodigoBarras)
                    .ToString();
        }


        // =====================================================
        // FILTROS
        // =====================================================

        private void AplicarFiltros()
        {
            IEnumerable<CarnetEmpleadoFila>
                resultado =
                    _filas;


            var texto =
                txtBuscar.Text
                    .Trim();


            if (!string.IsNullOrWhiteSpace(
                texto))
            {
                resultado =
                    resultado.Where(x =>
                        x.CodigoEmpleado.Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        x.NombreCompleto.Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        (
                            x.Puesto
                            ??
                            string.Empty
                        ).Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase));
            }


            if (cmbEstado.SelectedIndex ==
                1)
            {
                resultado =
                    resultado.Where(x =>
                        x.TieneCodigoBarras);
            }
            else if (
                cmbEstado.SelectedIndex ==
                2)
            {
                resultado =
                    resultado.Where(x =>
                        !x.TieneCodigoBarras);
            }


            dgCarnes.ItemsSource =
                resultado.ToList();
        }


        private void TxtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        private void CmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        // =====================================================
        // SELECCIÓN
        // =====================================================

        private void DgCarnes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            btnImprimirSeleccionados.IsEnabled =
                dgCarnes.SelectedItems
                    .Cast<CarnetEmpleadoFila>()
                    .Any(x =>
                        x.TieneCodigoBarras);


            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila)
            {
                LimpiarVistaPrevia();

                return;
            }


            MostrarVistaPrevia(
                fila);
        }


        // =====================================================
        // GENERAR
        // =====================================================

        private async void BtnGenerar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila)
            {
                return;
            }


            try
            {
                await _api
                    .GenerarAsync(
                        fila.EmpleadoID);


                await CargarAsync(
                    fila.EmpleadoID);
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // GENERAR FALTANTES
        // =====================================================

        private async void BtnGenerarFaltantes_Click(
            object sender,
            RoutedEventArgs e)
        {
            int faltantes =
                _filas.Count(x =>
                    !x.TieneCodigoBarras);


            if (faltantes == 0)
            {
                MessageBox.Show(
                    "Todos los empleados activos ya tienen un código de barras.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            var confirmar =
                MessageBox.Show(
                    $"Se generarán {faltantes} credenciales nuevas.\n\n¿Desea continuar?",
                    "Generar carnés",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                var generados =
                    await _api
                        .GenerarFaltantesAsync(
                            NegocioID);


                await CargarAsync();


                MessageBox.Show(
                    $"Se generaron {generados.Count} credenciales correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // REEMPLAZAR
        // =====================================================

        private async void BtnReemplazar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila)
            {
                return;
            }


            var confirmar =
                MessageBox.Show(
                    "La credencial actual quedará inactiva y dejará de funcionar en Marcaje.\n\n" +
                    "Se generará un código nuevo.\n\n¿Continuar?",
                    "Reemplazar credencial",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                await _api
                    .ReemplazarAsync(
                        fila.EmpleadoID);


                await CargarAsync(
                    fila.EmpleadoID);
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        private async void BtnDesactivar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila)
            {
                return;
            }


            var confirmar =
                MessageBox.Show(
                    "La credencial dejará de funcionar inmediatamente en Marcaje.\n\n¿Desea desactivarla?",
                    "Desactivar credencial",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                await _api
                    .DesactivarAsync(
                        fila.EmpleadoID);


                await CargarAsync(
                    fila.EmpleadoID);
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // REIMPRIMIR
        // =====================================================

        private void BtnReimprimir_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila
                ||
                !fila.TieneCodigoBarras)
            {
                return;
            }


            _impresion.Imprimir(
                new[]
                {
                    fila
                });
        }


        // =====================================================
        // IMPRIMIR SELECCIONADOS
        // =====================================================

        private void BtnImprimirSeleccionados_Click(
            object sender,
            RoutedEventArgs e)
        {
            var seleccionados =
                dgCarnes.SelectedItems
                    .Cast<CarnetEmpleadoFila>()
                    .Where(x =>
                        x.TieneCodigoBarras)
                    .ToList();


            _impresion.Imprimir(
                seleccionados);
        }


        // =====================================================
        // IMPRIMIR TODOS
        // =====================================================

        private void BtnImprimirTodos_Click(
            object sender,
            RoutedEventArgs e)
        {
            var carnets =
                _filas
                    .Where(x =>
                        x.TieneCodigoBarras)
                    .ToList();


            _impresion.Imprimir(
                carnets);
        }


        // =====================================================
        // OTRAS CREDENCIALES
        // =====================================================

        private async void BtnOtrasCredenciales_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgCarnes.SelectedItem
                is not CarnetEmpleadoFila fila)
            {
                return;
            }


            var ventana =
                new CredencialesEmpleadoWindow(
                    fila.EmpleadoID,
                    fila.NombreCompleto)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            ventana.ShowDialog();


            if (ventana.HuboCambios)
            {
                await CargarAsync(
                    fila.EmpleadoID);
            }
        }


        // =====================================================
        // PREVIEW
        // =====================================================

        private void MostrarVistaPrevia(
            CarnetEmpleadoFila fila)
        {
            txtEmpleadoSeleccionado.Text =
                fila.NombreCompleto;


            txtPreviewNombre.Text =
                fila.NombreCompleto
                    .ToUpperInvariant();


            txtPreviewPuesto.Text =
                fila.Puesto
                ??
                "Empleado";


            txtPreviewCodigoEmpleado.Text =
                $"Código: {fila.CodigoEmpleado}";


            txtEstadoCredencial.Text =
                fila.EstadoCredencial;


            txtFechaCredencial.Text =
                fila.FechaCredencial?
                    .ToString(
                        "dd/MM/yyyy")
                ??
                "--";


            btnGenerar.IsEnabled =
                !fila.TieneCodigoBarras;


            btnReimprimir.IsEnabled =
                fila.TieneCodigoBarras;


            btnReemplazar.IsEnabled =
                fila.TieneCodigoBarras;


            btnDesactivar.IsEnabled =
                fila.TieneCodigoBarras;


            btnOtrasCredenciales.IsEnabled =
                true;


            if (!fila.TieneCodigoBarras)
            {
                imgCodigoBarras.Source =
                    null;


                txtPreviewCodigoCredencial.Text =
                    "SIN CREDENCIAL";


                return;
            }


            imgCodigoBarras.Source =
                _codigoBarras
                    .GenerarCode128(
                        fila.CodigoCredencial!);


            txtPreviewCodigoCredencial.Text =
                fila.CodigoCredencial;
        }


        private void LimpiarVistaPrevia()
        {
            txtEmpleadoSeleccionado.Text =
                "Selecciona un empleado";


            txtPreviewNombre.Text =
                "NOMBRE DEL EMPLEADO";


            txtPreviewPuesto.Text =
                "Puesto";


            txtPreviewCodigoEmpleado.Text =
                "Código: --";


            txtEstadoCredencial.Text =
                "--";


            txtFechaCredencial.Text =
                "--";


            txtPreviewCodigoCredencial.Text =
                "SIN CREDENCIAL";


            imgCodigoBarras.Source =
                null;


            btnGenerar.IsEnabled =
                false;

            btnReimprimir.IsEnabled =
                false;

            btnReemplazar.IsEnabled =
                false;

            btnDesactivar.IsEnabled =
                false;

            btnOtrasCredenciales.IsEnabled =
                false;
        }


        private static void MostrarError(
            string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}