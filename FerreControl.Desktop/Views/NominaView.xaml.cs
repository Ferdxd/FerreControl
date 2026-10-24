using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.Nomina;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class NominaView
        : UserControl
    {
        private readonly NominaApiService
            _nominaApi;

        private readonly EmpleadoApiService
            _empleadoApi;


        private List<NominaDto>
            _nominas;

        private List<EmpleadoDto>
            _empleados;

        private List<NominaDetalleFila>
            _filasDetalle;


        private NominaDto?
            _nominaSeleccionada;


        private const int NegocioID = 1;


        public NominaView()
        {
            InitializeComponent();


            _nominaApi =
                new NominaApiService();


            _empleadoApi =
                new EmpleadoApiService();


            _nominas =
                new List<NominaDto>();


            _empleados =
                new List<EmpleadoDto>();


            _filasDetalle =
                new List<NominaDetalleFila>();


            cmbEstadoNomina.SelectedIndex =
                0;


            Loaded +=
                NominaView_Loaded;
        }


        // =====================================================
        // CARGA INICIAL
        // =====================================================

        private async void NominaView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarDatosAsync();
        }


        private async Task CargarDatosAsync()
        {
            try
            {
                _empleados =
                    (await _empleadoApi
                        .GetObtenerTodos())
                    .Where(x =>
                        x.NegocioID == NegocioID)
                    .ToList();


                _nominas =
                    await _nominaApi
                        .ObtenerPorNegocioAsync(
                            NegocioID);


                AplicarFiltros();


                LimpiarDetalle();


                if (lstNominas.Items.Count > 0)
                {
                    lstNominas.SelectedIndex =
                        0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar las nóminas.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // FILTROS
        // =====================================================

        private void AplicarFiltros()
        {
            IEnumerable<NominaDto> resultado =
                _nominas;


            var texto =
                txtBuscarNomina.Text
                    .Trim()
                    .ToLower();


            if (!string.IsNullOrWhiteSpace(
                texto))
            {
                resultado =
                    resultado.Where(x =>
                        x.Estado
                            .ToLower()
                            .Contains(texto)

                        ||

                        x.FechaInicioPeriodo
                            .ToString("dd/MM/yyyy")
                            .Contains(texto)

                        ||

                        x.FechaFinPeriodo
                            .ToString("dd/MM/yyyy")
                            .Contains(texto));
            }


            if (cmbEstadoNomina.SelectedIndex > 0)
            {
                var item =
                    cmbEstadoNomina.SelectedItem
                        as ComboBoxItem;


                var estado =
                    item?.Content?
                        .ToString();


                if (!string.IsNullOrWhiteSpace(
                    estado))
                {
                    resultado =
                        resultado.Where(x =>
                            string.Equals(
                                x.Estado,
                                estado,
                                StringComparison.OrdinalIgnoreCase));
                }
            }


            lstNominas.ItemsSource =
                resultado
                    .OrderByDescending(x =>
                        x.FechaInicioPeriodo)
                    .ThenByDescending(x =>
                        x.ID)
                    .ToList();
        }


        private void TxtBuscarNomina_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        private void CmbEstadoNomina_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        // =====================================================
        // SELECCIÓN NÓMINA
        // =====================================================

        private async void LstNominas_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (lstNominas.SelectedItem
                is not NominaDto nomina)
            {
                _nominaSeleccionada =
                    null;

                LimpiarDetalle();

                return;
            }


            _nominaSeleccionada =
                nomina;


            txtPeriodoSeleccionado.Text =
                $"{nomina.FechaInicioPeriodo:dd/MM/yyyy} - " +
                $"{nomina.FechaFinPeriodo:dd/MM/yyyy}";


            txtEstadoSeleccionado.Text =
                $"Estado: {nomina.Estado}";


            txtFechaInicio.Text =
                nomina.FechaInicioPeriodo
                    .ToString("dd/MM/yyyy");


            txtFechaFin.Text =
                nomina.FechaFinPeriodo
                    .ToString("dd/MM/yyyy");


            await CargarDetallesAsync(
                nomina.ID);
        }


        // =====================================================
        // DETALLE NÓMINA
        // =====================================================

        private async Task CargarDetallesAsync(
            int nominaId)
        {
            try
            {
                var detalles =
                    await _nominaApi
                        .ObtenerDetallesAsync(
                            nominaId);


                _filasDetalle =
                    detalles.Select(detalle =>
                    {
                        var empleado =
                            _empleados
                                .FirstOrDefault(x =>
                                    x.ID ==
                                    detalle.EmpleadoID);


                        return new NominaDetalleFila
                        {
                            ID =
                                detalle.ID,

                            EmpleadoID =
                                detalle.EmpleadoID,

                            CodigoEmpleado =
                                empleado?.Codigo
                                ??
                                detalle.EmpleadoID
                                    .ToString(),

                            Empleado =
                                empleado == null
                                    ?
                                    $"Empleado {detalle.EmpleadoID}"
                                    :
                                    $"{empleado.Nombres} {empleado.Apellidos}",

                            SalarioBase =
                                detalle.SalarioBase,

                            Bonificaciones =
                                detalle.Bonificaciones,

                            HorasExtra =
                                detalle.HorasExtra,

                            Adelantos =
                                detalle.Adelantos,

                            OtrosDescuentos =
                                detalle.OtrosDescuentos,

                            TotalPagar =
                                detalle.TotalPagar,

                            TotalPagado =
                                detalle.TotalPagado,

                            Pendiente =
                                detalle.Pendiente
                        };
                    })
                    .ToList();


                dgNominaDetalle.ItemsSource =
                    _filasDetalle;


                txtCantidadEmpleados.Text =
                    _filasDetalle.Count
                        .ToString();


                ActualizarTotales();


                dgNominaDetalle.SelectedItem =
                    null;


                ActualizarDisponibilidadOperaciones();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cargar el detalle de la nómina.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void ActualizarTotales()
        {
            decimal total =
                _filasDetalle.Sum(x =>
                    x.TotalPagar);


            decimal pagado =
                _filasDetalle.Sum(x =>
                    x.TotalPagado);


            decimal pendiente =
                _filasDetalle.Sum(x =>
                    x.Pendiente);


            txtTotalNomina.Text =
                $"Q {total:N2}";


            txtTotalPagado.Text =
                $"Q {pagado:N2}";


            txtTotalPendiente.Text =
                $"Q {pendiente:N2}";
        }


        // =====================================================
        // SELECCIÓN EMPLEADO
        // =====================================================

        private void DgNominaDetalle_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            ActualizarDisponibilidadOperaciones();
        }
        private void ActualizarDisponibilidadOperaciones()
        {
            // =====================================================
            // DESHABILITAR TODO
            // =====================================================

            btnAgregarEmpleado.IsEnabled =
                false;

            btnDarAdelanto.IsEnabled =
                false;

            btnHistorialAdelantos.IsEnabled =
                false;

            btnGenerarNomina.IsEnabled =
                false;

            btnRegistrarPago.IsEnabled =
                false;

            btnVerPagos.IsEnabled =
                false;

            btnCerrarNomina.IsEnabled =
                false;


            if (_nominaSeleccionada == null)
                return;


            var detalleSeleccionado =
                dgNominaDetalle.SelectedItem
                as NominaDetalleFila;


            bool borrador =
                string.Equals(
                    _nominaSeleccionada.Estado,
                    "BORRADOR",
                    StringComparison.OrdinalIgnoreCase);


            bool generada =
                string.Equals(
                    _nominaSeleccionada.Estado,
                    "GENERADA",
                    StringComparison.OrdinalIgnoreCase);


            bool cerrada =
                string.Equals(
                    _nominaSeleccionada.Estado,
                    "CERRADA",
                    StringComparison.OrdinalIgnoreCase);


            // =====================================================
            // ACCIONES DEL EMPLEADO
            // =====================================================

            if (detalleSeleccionado != null)
            {
                /*
                 * Registrar un adelanto es independiente
                 * de la nómina seleccionada.
                 *
                 * Incluso desde una nómina GENERADA/CERRADA
                 * se puede registrar un nuevo adelanto.
                 *
                 * Lo que NO se podrá hacer es aplicarlo
                 * a esa nómina.
                 */

                btnDarAdelanto.IsEnabled =
                    true;


                btnHistorialAdelantos.IsEnabled =
                    true;
            }


            // =====================================================
            // CERRADA
            // =====================================================

            if (cerrada)
            {
                if (detalleSeleccionado != null)
                {
                    btnVerPagos.IsEnabled =
                        detalleSeleccionado.TotalPagado > 0;
                }

                return;
            }


            // =====================================================
            // BORRADOR
            // =====================================================

            if (borrador)
            {
                btnAgregarEmpleado.IsEnabled =
                    true;


                btnGenerarNomina.IsEnabled =
                    _filasDetalle.Count > 0;


                return;
            }


            // =====================================================
            // GENERADA
            // =====================================================

            if (generada)
            {
                bool tieneDetalles =
                    _filasDetalle.Count > 0;


                bool todoPagado =
                    tieneDetalles
                    &&
                    _filasDetalle.All(x =>
                        x.Pendiente <= 0);


                btnCerrarNomina.IsEnabled =
                    todoPagado;


                if (detalleSeleccionado != null)
                {
                    btnRegistrarPago.IsEnabled =
                        detalleSeleccionado.Pendiente > 0;


                    btnVerPagos.IsEnabled =
                        detalleSeleccionado.TotalPagado > 0;
                }
            }
        }

        // =====================================================
        // GENERAR NÓMINA
        // =====================================================

        private async void BtnGenerarNomina_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            if (_filasDetalle.Count == 0)
            {
                MessageBox.Show(
                    "Debe agregar por lo menos un empleado antes de generar la nómina.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var confirmar =
                MessageBox.Show(
                    "Al generar la nómina ya no podrá modificar empleados, adelantos ni descuentos.\n\n¿Desea continuar?",
                    "Generar nómina",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                await _nominaApi
                    .ActualizarEstadoAsync(
                        _nominaSeleccionada.ID,
                        "GENERADA");


                MessageBox.Show(
                    "La nómina fue generada correctamente.\n\nYa puede registrar los pagos.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                await RecargarNominasAsync(
                    _nominaSeleccionada.ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo generar la nómina.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // CERRAR NÓMINA
        // =====================================================

        private async void BtnCerrarNomina_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            if (_filasDetalle.Any(x =>
                x.Pendiente > 0))
            {
                MessageBox.Show(
                    "No puede cerrar la nómina mientras existan pagos pendientes.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var confirmar =
                MessageBox.Show(
                    "Una nómina cerrada quedará únicamente para consulta.\n\n¿Desea cerrarla?",
                    "Cerrar nómina",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                await _nominaApi
                    .ActualizarEstadoAsync(
                        _nominaSeleccionada.ID,
                        "CERRADA");


                MessageBox.Show(
                    "La nómina fue cerrada correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                await RecargarNominasAsync(
                    _nominaSeleccionada.ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cerrar la nómina.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // RECARGAR
        // =====================================================

        private async Task RecargarNominasAsync(
            int seleccionarNominaId)
        {
            _nominas =
                await _nominaApi
                    .ObtenerPorNegocioAsync(
                        NegocioID);


            AplicarFiltros();


            var seleccionar =
                lstNominas.Items
                    .Cast<NominaDto>()
                    .FirstOrDefault(x =>
                        x.ID ==
                        seleccionarNominaId);


            if (seleccionar != null)
            {
                lstNominas.SelectedItem =
                    seleccionar;
            }
            else
            {
                LimpiarDetalle();
            }
        }


        // =====================================================
        // NUEVA NÓMINA
        // =====================================================

        private async void BtnNuevaNomina_Click(
         object sender,
         RoutedEventArgs e)
        {
            var ventana =
                new NominaFormWindow(
                    NegocioID)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                await RecargarNominasAsync(
                    ventana.NominaCreadaId);
            }
        }


        // =====================================================
        // AGREGAR EMPLEADO
        // =====================================================

        private async void BtnAgregarEmpleado_Click(
       object sender,
       RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            var empleadosExistentes =
                _filasDetalle
                    .Select(x =>
                        x.EmpleadoID)
                    .ToList();


            var ventana =
                new AgregarEmpleadoNominaWindow(
                    _nominaSeleccionada.ID,
                    NegocioID,
                    empleadosExistentes)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                await CargarDetallesAsync(
                    _nominaSeleccionada.ID);
            }
        }
 
        // =====================================================
        // REGISTRAR PAGO
        // =====================================================
        private async void BtnRegistrarPago_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            if (dgNominaDetalle.SelectedItem
                is not NominaDetalleFila detalle)
            {
                return;
            }


            var ventana =
                new PagoNominaFormWindow(
                    detalle)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                await CargarDetallesAsync(
                    _nominaSeleccionada.ID);
            }
        }

        // =====================================================
        // HISTORIAL PAGOS
        // =====================================================

        private async void BtnVerPagos_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            if (dgNominaDetalle.SelectedItem
                is not NominaDetalleFila detalle)
            {
                return;
            }


            var ventana =
                new PagosNominaWindow(
                    detalle)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            ventana.ShowDialog();


            if (ventana.HuboCambios)
            {
                await CargarDetallesAsync(
                    _nominaSeleccionada.ID);
            }
        }

        // =====================================================
        // LIMPIAR
        // =====================================================

        private void LimpiarDetalle()
        {
            _nominaSeleccionada =
                null;


            _filasDetalle =
                new List<NominaDetalleFila>();


            txtPeriodoSeleccionado.Text =
                "Selecciona una nómina";


            txtEstadoSeleccionado.Text =
                "--";


            txtFechaInicio.Text =
                "--";


            txtFechaFin.Text =
                "--";


            txtCantidadEmpleados.Text =
                "--";


            txtTotalNomina.Text =
                "Q 0.00";


            txtTotalPagado.Text =
                "Q 0.00";


            txtTotalPendiente.Text =
                "Q 0.00";


            dgNominaDetalle.ItemsSource =
                null;


            ActualizarDisponibilidadOperaciones();
        }

        private async void BtnDarAdelanto_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (_nominaSeleccionada == null)
                return;


            if (dgNominaDetalle.SelectedItem
                is not NominaDetalleFila detalle)
            {
                MessageBox.Show(
                    "Seleccione primero un empleado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            bool permiteAplicarNomina =
                string.Equals(
                    _nominaSeleccionada.Estado,
                    "BORRADOR",
                    StringComparison.OrdinalIgnoreCase);


            var ventana =
                new AdelantoFormWindow(
                    detalle,
                    permiteAplicarNomina)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                /*
                 * Solo recargamos el detalle si realmente
                 * se aplicó dinero a esta nómina.
                 */

                if (ventana.HuboCambiosNomina)
                {
                    await CargarDetallesAsync(
                        _nominaSeleccionada.ID);
                }
            }
        }

        private void BtnHistorialAdelantos_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (dgNominaDetalle.SelectedItem
                is not NominaDetalleFila detalle)
            {
                return;
            }


            var ventana =
                new AdelantosEmpleadoFormWindow(
                    detalle.EmpleadoID,
                    detalle.Empleado)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            ventana.ShowDialog();
        }

    }
}