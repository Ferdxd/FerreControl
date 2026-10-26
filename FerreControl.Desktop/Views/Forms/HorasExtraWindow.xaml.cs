using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Desktop.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms;

public partial class HorasExtraWindow
    : Window
{
    private const int NegocioID = 1;

    private readonly HoraExtraApiService _horaExtraApi;
    private readonly EmpleadoApiService _empleadoApi;

    private List<ReglaHoraExtraDto> _reglas;
    private List<EmpleadoDto> _empleados;

    public HorasExtraWindow()
    {
        InitializeComponent();

        _horaExtraApi =
            new HoraExtraApiService();

        _empleadoApi =
            new EmpleadoApiService();

        _reglas =
            new List<ReglaHoraExtraDto>();

        _empleados =
            new List<EmpleadoDto>();

        dpFechaInicioRegla.SelectedDate =
            DateTime.Today;

        dpHasta.SelectedDate =
            DateTime.Today;

        dpDesde.SelectedDate =
            DateTime.Today.AddDays(-6);

        Loaded +=
            HorasExtraWindow_Loaded;
    }

    private async void HorasExtraWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await CargarTodoAsync();
    }

    private async Task CargarTodoAsync()
    {
        try
        {
            _empleados =
                (await _empleadoApi.GetObtenerTodos())
                .Where(x =>
                    x.NegocioID == NegocioID &&
                    x.Estado)
                .OrderBy(x => x.Nombres)
                .ThenBy(x => x.Apellidos)
                .ToList();

            cmbEmpleado.ItemsSource =
                _empleados;

            if (_empleados.Count > 0 &&
                cmbEmpleado.SelectedItem == null)
            {
                cmbEmpleado.SelectedIndex = 0;
            }

            await CargarReglasAsync();
            await CargarRegistrosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo cargar la gestión de horas extra.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // REGLAS
    // =========================================================

    private async Task CargarReglasAsync()
    {
        _reglas =
            await _horaExtraApi
                .ObtenerReglasAsync(
                    NegocioID);

        dgReglas.ItemsSource =
            _reglas;

        cmbRegla.ItemsSource =
            _reglas
                .Where(x => x.Estado)
                .ToList();

        if (cmbRegla.Items.Count > 0 &&
            cmbRegla.SelectedItem == null)
        {
            cmbRegla.SelectedIndex = 0;
        }
    }

    private async void BtnCrearRegla_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            txtNombreRegla.Text))
        {
            MessageBox.Show(
                "Ingrese un nombre para la regla.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!decimal.TryParse(
            txtTarifaRegla.Text,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal tarifa)
            ||
            tarifa <= 0)
        {
            MessageBox.Show(
                "Ingrese una tarifa válida por hora extra.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await _horaExtraApi
                .CrearReglaAsync(
                    new CrearReglaHoraExtraDto
                    {
                        NegocioID = NegocioID,
                        Nombre = txtNombreRegla.Text.Trim(),
                        TarifaHora = tarifa,
                        RequiereAprobacion =
                            chkRequiereAprobacion.IsChecked == true,
                        CuentaEntradaAnticipada =
                            chkEntradaAnticipada.IsChecked == true
                    });

            txtNombreRegla.Text =
                string.Empty;

            txtTarifaRegla.Text =
                string.Empty;

            chkRequiereAprobacion.IsChecked =
                true;

            chkEntradaAnticipada.IsChecked =
                false;

            await CargarReglasAsync();

            MessageBox.Show(
                "Regla creada correctamente.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo crear la regla.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // ASIGNACIÓN
    // =========================================================

    private async void CmbEmpleado_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        await MostrarReglaActualAsync();
    }

    private async Task MostrarReglaActualAsync()
    {
        if (cmbEmpleado.SelectedItem
            is not EmpleadoDto empleado)
        {
            txtReglaActual.Text =
                "Seleccione un empleado";

            txtReglaActualDetalle.Text =
                string.Empty;

            return;
        }

        try
        {
            var actual =
                await _horaExtraApi
                    .ObtenerActualAsync(
                        empleado.ID);

            if (actual == null)
            {
                txtReglaActual.Text =
                    "Sin regla asignada";

                txtReglaActualDetalle.Text =
                    "Este empleado todavía no genera pago automático de horas extra.";

                return;
            }

            txtReglaActual.Text =
                actual.ReglaNombre;

            txtReglaActualDetalle.Text =
                $"Q {actual.TarifaHora:N2}/h · Desde {actual.FechaInicio:dd/MM/yyyy}";
        }
        catch (Exception ex)
        {
            txtReglaActual.Text =
                "No se pudo consultar";

            txtReglaActualDetalle.Text =
                ex.Message;
        }
    }

    private async void BtnAsignarRegla_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (cmbEmpleado.SelectedItem
            is not EmpleadoDto empleado)
        {
            MessageBox.Show(
                "Seleccione un empleado.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (cmbRegla.SelectedItem
            is not ReglaHoraExtraDto regla)
        {
            MessageBox.Show(
                "Seleccione una regla.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!dpFechaInicioRegla.SelectedDate.HasValue)
        {
            MessageBox.Show(
                "Seleccione la fecha de inicio.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var confirmar =
            MessageBox.Show(
                $"Empleado: {empleado.Nombres} {empleado.Apellidos}\n" +
                $"Regla: {regla.Nombre}\n" +
                $"Tarifa: Q {regla.TarifaHora:N2}/h\n" +
                $"Desde: {dpFechaInicioRegla.SelectedDate.Value:dd/MM/yyyy}\n\n" +
                "¿Desea asignar esta regla?",
                "Asignar horas extra",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

        if (confirmar !=
            MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _horaExtraApi
                .AsignarAsync(
                    empleado.ID,
                    new AsignarReglaHoraExtraDto
                    {
                        ReglaHoraExtraID =
                            regla.ID,

                        FechaInicio =
                            dpFechaInicioRegla
                                .SelectedDate
                                .Value.Date
                    });

            await MostrarReglaActualAsync();

            MessageBox.Show(
                "Regla asignada correctamente.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo asignar la regla.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // CÁLCULO
    // =========================================================

    private async void BtnCalcular_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ValidarFechas(
            out DateTime desde,
            out DateTime hasta))
        {
            return;
        }

        try
        {
            dgRegistros.ItemsSource =
                await _horaExtraApi
                    .CalcularAsync(
                        new CalcularHorasExtraDto
                        {
                            NegocioID = NegocioID,
                            Desde = desde,
                            Hasta = hasta
                        });

            LimpiarDecision();

            MessageBox.Show(
                "Horas extra recalculadas desde las marcaciones de asistencia.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron calcular las horas extra.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnActualizar_Click(
        object sender,
        RoutedEventArgs e)
    {
        await CargarRegistrosAsync();
    }

    private async Task CargarRegistrosAsync()
    {
        if (!ValidarFechas(
            out DateTime desde,
            out DateTime hasta,
            mostrarMensaje: false))
        {
            return;
        }

        dgRegistros.ItemsSource =
            await _horaExtraApi
                .ObtenerRegistrosAsync(
                    NegocioID,
                    desde,
                    hasta);

        LimpiarDecision();
    }

    // =========================================================
    // APROBAR / RECHAZAR
    // =========================================================

    private void DgRegistros_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (dgRegistros.SelectedItem
            is not RegistroHoraExtraDto registro)
        {
            LimpiarDecision();
            return;
        }

        bool puedeDecidir =
            registro.Estado == "PENDIENTE";

        txtMinutosAprobados.IsEnabled =
            puedeDecidir;

        txtObservacionDecision.IsEnabled =
            puedeDecidir;

        btnAprobar.IsEnabled =
            puedeDecidir;

        btnRechazar.IsEnabled =
            puedeDecidir;

        txtMinutosAprobados.Text =
            registro.MinutosCalculados.ToString();

        txtObservacionDecision.Text =
            registro.Observacion
            ?? string.Empty;

        if (registro.ExcedeLimite12Horas)
        {
            MessageBox.Show(
                "Advertencia: la jornada calculada supera 12 horas de trabajo conocidas. Revise las marcaciones antes de aprobar.",
                "Revisión requerida",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private async void BtnAprobar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (dgRegistros.SelectedItem
            is not RegistroHoraExtraDto registro)
        {
            return;
        }

        if (!int.TryParse(
            txtMinutosAprobados.Text,
            out int minutos)
            ||
            minutos < 0)
        {
            MessageBox.Show(
                "Ingrese una cantidad válida de minutos.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await _horaExtraApi
                .AprobarAsync(
                    registro.ID,
                    new AprobarHoraExtraDto
                    {
                        MinutosAprobados =
                            minutos,

                        Observacion =
                            string.IsNullOrWhiteSpace(
                                txtObservacionDecision.Text)
                            ?
                            null
                            :
                            txtObservacionDecision.Text.Trim()
                    });

            await CargarRegistrosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo aprobar el registro.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void BtnRechazar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (dgRegistros.SelectedItem
            is not RegistroHoraExtraDto registro)
        {
            return;
        }

        try
        {
            await _horaExtraApi
                .RechazarAsync(
                    registro.ID,
                    new RechazarHoraExtraDto
                    {
                        Observacion =
                            string.IsNullOrWhiteSpace(
                                txtObservacionDecision.Text)
                            ?
                            null
                            :
                            txtObservacionDecision.Text.Trim()
                    });

            await CargarRegistrosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo rechazar el registro.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void LimpiarDecision()
    {
        txtMinutosAprobados.Text =
            string.Empty;

        txtObservacionDecision.Text =
            string.Empty;

        txtMinutosAprobados.IsEnabled =
            false;

        txtObservacionDecision.IsEnabled =
            false;

        btnAprobar.IsEnabled =
            false;

        btnRechazar.IsEnabled =
            false;
    }

    private bool ValidarFechas(
        out DateTime desde,
        out DateTime hasta,
        bool mostrarMensaje = true)
    {
        desde = default;
        hasta = default;

        if (!dpDesde.SelectedDate.HasValue ||
            !dpHasta.SelectedDate.HasValue)
        {
            if (mostrarMensaje)
            {
                MessageBox.Show(
                    "Seleccione el período a consultar.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            return false;
        }

        desde =
            dpDesde.SelectedDate.Value.Date;

        hasta =
            dpHasta.SelectedDate.Value.Date;

        if (desde > hasta)
        {
            if (mostrarMensaje)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser posterior a la fecha final.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            return false;
        }

        return true;
    }

    private void BtnCerrar_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
