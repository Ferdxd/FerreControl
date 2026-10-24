using FerreControl.Application.DTOs.Adelantos;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class AdelantoFormWindow : Window
    {
        private readonly AdelantoApiService
            _adelantoApi;

        private readonly NominaDetalleFila
            _detalle;

        private readonly bool
            _permiteAplicarNomina;


        public int AdelantoCreadoId
        {
            get;
            private set;
        }


        public bool HuboCambiosNomina
        {
            get;
            private set;
        }


        public AdelantoFormWindow(
            NominaDetalleFila detalle,
            bool permiteAplicarNomina)
        {
            InitializeComponent();


            _detalle =
                detalle;


            _permiteAplicarNomina =
                permiteAplicarNomina;


            _adelantoApi =
                new AdelantoApiService();


            // =====================================================
            // EMPLEADO
            // =====================================================

            txtEmpleado.Text =
                $"{detalle.CodigoEmpleado} - {detalle.Empleado}";


            // =====================================================
            // FECHA / HORA ACTUAL
            // =====================================================

            var ahora =
                DateTime.Now;


            dpFecha.SelectedDate =
                ahora.Date;


            txtHora.Text =
                ahora.ToString(
                    "HH:mm");


            cmbMetodo.SelectedIndex =
                0;


            // =====================================================
            // ESTADO DE LA NÓMINA
            // =====================================================

            if (_permiteAplicarNomina)
            {
                txtEstadoNomina.Text =
                    "Nómina en BORRADOR: puede aplicar adelantos a este período.";


                chkAplicarNomina.IsEnabled =
                    true;


                chkAplicarNomina.IsChecked =
                    true;
            }
            else
            {
                txtEstadoNomina.Text =
                    "La nómina no está en BORRADOR. Puede registrar adelantos, " +
                    "pero no aplicarlos a este período.";


                chkAplicarNomina.IsChecked =
                    false;


                chkAplicarNomina.IsEnabled =
                    false;


                txtMontoAplicarNuevo.IsEnabled =
                    false;
            }


            Loaded +=
                AdelantoFormWindow_Loaded;
        }


        // =====================================================
        // CARGA
        // =====================================================

        private async void AdelantoFormWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarPendientesAsync();
        }


        private async Task CargarPendientesAsync()
        {
            try
            {
                var pendientes =
                    await _adelantoApi
                        .ObtenerPendientesPorEmpleadoAsync(
                            _detalle.EmpleadoID);


                dgPendientes.ItemsSource =
                    pendientes;


                bool tienePendientes =
                    pendientes.Count > 0;


                dgPendientes.Visibility =
                    tienePendientes
                        ?
                        Visibility.Visible
                        :
                        Visibility.Collapsed;


                txtSinPendientes.Visibility =
                    tienePendientes
                        ?
                        Visibility.Collapsed
                        :
                        Visibility.Visible;


                txtMontoAplicarPendiente.Text =
                    string.Empty;


                txtMontoAplicarPendiente.IsEnabled =
                    false;


                btnAplicarPendiente.IsEnabled =
                    false;


                dgPendientes.SelectedItem =
                    null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los adelantos pendientes.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // SELECCIÓN DE ADELANTO PENDIENTE
        // =====================================================

        private void DgPendientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgPendientes.SelectedItem
                is not AdelantoEmpleadoDto adelanto)
            {
                txtMontoAplicarPendiente.Text =
                    string.Empty;


                txtMontoAplicarPendiente.IsEnabled =
                    false;


                btnAplicarPendiente.IsEnabled =
                    false;


                return;
            }


            txtMontoAplicarPendiente.Text =
                adelanto.SaldoPendiente
                    .ToString("0.00");


            txtMontoAplicarPendiente.IsEnabled =
                _permiteAplicarNomina;


            btnAplicarPendiente.IsEnabled =
                _permiteAplicarNomina;
        }


        // =====================================================
        // APLICAR UN ADELANTO PENDIENTE
        // =====================================================

        private async void BtnAplicarPendiente_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!_permiteAplicarNomina)
            {
                MessageBox.Show(
                    "Solo se pueden aplicar adelantos a una nómina en BORRADOR.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (dgPendientes.SelectedItem
                is not AdelantoEmpleadoDto adelanto)
            {
                MessageBox.Show(
                    "Seleccione un adelanto pendiente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!decimal.TryParse(
                txtMontoAplicarPendiente.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
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
                adelanto.SaldoPendiente)
            {
                MessageBox.Show(
                    $"El adelanto solamente tiene Q {adelanto.SaldoPendiente:N2} pendientes.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                btnAplicarPendiente.IsEnabled =
                    false;


                var dto =
                    new CrearAdelantoAplicacionDto
                    {
                        NominaDetalleID =
                            _detalle.ID,

                        Monto =
                            monto,

                        Observacion =
                            "Aplicación realizada desde la gestión de nómina."
                    };


                await _adelantoApi
                    .AplicarAsync(
                        adelanto.ID,
                        dto);


                HuboCambiosNomina =
                    true;


                MessageBox.Show(
                    $"Adelanto aplicado correctamente.\n\n" +
                    $"Monto descontado: Q {monto:N2}",
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
                btnAplicarPendiente.IsEnabled =
                    true;
            }
        }


        // =====================================================
        // CHECKBOX APLICAR NUEVO ADELANTO
        // =====================================================

        private void ChkAplicarNomina_Changed(
            object sender,
            RoutedEventArgs e)
        {
            if (txtMontoAplicarNuevo == null)
                return;


            bool aplicar =
                _permiteAplicarNomina
                &&
                chkAplicarNomina.IsChecked == true;


            txtMontoAplicarNuevo.IsEnabled =
                aplicar;


            if (aplicar
                &&
                decimal.TryParse(
                    txtMonto.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal monto)
                &&
                monto > 0)
            {
                txtMontoAplicarNuevo.Text =
                    monto.ToString("0.00");
            }
        }


        // =====================================================
        // SINCRONIZAR MONTO NUEVO
        // =====================================================

        private void TxtMonto_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (txtMontoAplicarNuevo == null)
                return;


            if (!_permiteAplicarNomina)
                return;


            if (chkAplicarNomina.IsChecked != true)
                return;


            if (decimal.TryParse(
                txtMonto.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal monto)
                &&
                monto > 0)
            {
                txtMontoAplicarNuevo.Text =
                    monto.ToString("0.00");
            }
            else
            {
                txtMontoAplicarNuevo.Text =
                    string.Empty;
            }
        }


        // =====================================================
        // REGISTRAR NUEVO ADELANTO
        // =====================================================

        private async void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            // =================================================
            // MONTO
            // =================================================

            if (!decimal.TryParse(
                txtMonto.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
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


            // =================================================
            // FECHA
            // =================================================

            if (!dpFecha.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha del adelanto.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // HORA
            // =================================================

            if (!TimeOnly.TryParseExact(
                txtHora.Text.Trim(),
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out TimeOnly hora))
            {
                MessageBox.Show(
                    "Ingrese una hora válida en formato HH:mm.\n\n" +
                    "Ejemplo: 15:25",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var fecha =
                dpFecha.SelectedDate
                    .Value.Date;


            var fechaHoraAdelanto =
                fecha.Add(
                    hora.ToTimeSpan());


            if (fechaHoraAdelanto >
                DateTime.Now.AddMinutes(1))
            {
                MessageBox.Show(
                    "La fecha y hora del adelanto no pueden ser futuras.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // MÉTODO
            // =================================================

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


            bool aplicarEnNomina =
                _permiteAplicarNomina
                &&
                chkAplicarNomina.IsChecked == true;


            decimal montoAplicar =
                0;


            if (aplicarEnNomina)
            {
                if (!decimal.TryParse(
                    txtMontoAplicarNuevo.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out montoAplicar)
                    ||
                    montoAplicar <= 0)
                {
                    MessageBox.Show(
                        "Ingrese un monto válido para descontar de la nómina.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                if (montoAplicar >
                    monto)
                {
                    MessageBox.Show(
                        "El monto a descontar no puede ser mayor " +
                        "que el adelanto entregado.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }
            }


            btnGuardar.IsEnabled =
                false;


            AdelantoEmpleadoDto resultado;


            // =================================================
            // 1. REGISTRAR ADELANTO
            // =================================================

            try
            {
                var dto =
                    new CrearAdelantoEmpleadoDto
                    {
                        Monto =
                            monto,

                        FechaAdelanto =
                            fechaHoraAdelanto,

                        MetodoEntrega =
                            metodoItem.Content?
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


                resultado =
                    await _adelantoApi
                        .CrearAsync(
                            _detalle.EmpleadoID,
                            dto);


                AdelantoCreadoId =
                    resultado.ID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo registrar el adelanto.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                btnGuardar.IsEnabled =
                    true;


                return;
            }


            // =================================================
            // 2. SI NO SE APLICARÁ A ESTA NÓMINA
            // =================================================

            if (!aplicarEnNomina)
            {
                MessageBox.Show(
                    $"Adelanto registrado correctamente.\n\n" +
                    $"Monto entregado: Q {resultado.Monto:N2}\n" +
                    $"Fecha y hora: {resultado.FechaAdelanto:dd/MM/yyyy HH:mm}\n" +
                    $"Método: {resultado.MetodoEntrega}\n\n" +
                    $"El adelanto quedó pendiente para una nómina posterior.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;


                return;
            }


            // =================================================
            // 3. APLICAR AUTOMÁTICAMENTE A ESTA NÓMINA
            // =================================================

            try
            {
                var aplicacionDto =
                    new CrearAdelantoAplicacionDto
                    {
                        NominaDetalleID =
                            _detalle.ID,

                        Monto =
                            montoAplicar,

                        Observacion =
                            "Aplicación automática al registrar el adelanto."
                    };


                await _adelantoApi
                    .AplicarAsync(
                        resultado.ID,
                        aplicacionDto);


                HuboCambiosNomina =
                    true;


                decimal restante =
                    monto
                    -
                    montoAplicar;


                string estadoSaldo =
                    restante <= 0
                        ?
                        "El adelanto quedó totalmente liquidado."
                        :
                        $"Saldo pendiente del adelanto: Q {restante:N2}";


                MessageBox.Show(
                    $"Adelanto registrado y aplicado correctamente.\n\n" +
                    $"Entregado: Q {monto:N2}\n" +
                    $"Descontado en esta nómina: Q {montoAplicar:N2}\n" +
                    $"{estadoSaldo}\n\n" +
                    $"Fecha y hora: {fechaHoraAdelanto:dd/MM/yyyy HH:mm}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                /*
                 * IMPORTANTE:
                 * El adelanto ya fue creado.
                 *
                 * Si falla la aplicación NO debemos decir
                 * que todo falló, porque el dinero entregado
                 * sí quedó registrado.
                 */

                MessageBox.Show(
                    $"El adelanto fue registrado correctamente, " +
                    $"pero no pudo aplicarse a esta nómina.\n\n" +
                    $"El adelanto quedó PENDIENTE y no se perdió.\n\n" +
                    $"{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);


                await CargarPendientesAsync();


                btnGuardar.IsEnabled =
                    false;
            }
        }


        // =====================================================
        // CANCELAR
        // =====================================================

        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult =
                false;
        }
    }
}