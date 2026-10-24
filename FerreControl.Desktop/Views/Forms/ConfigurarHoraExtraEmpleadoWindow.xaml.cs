using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms;

public partial class ConfigurarHoraExtraEmpleadoWindow
    : Window
{
    private readonly EmpleadoDto _empleado;
    private readonly int _negocioId;
    private readonly HoraExtraApiService _horaExtraApi;

    private List<ReglaHoraExtraDto> _reglas;
    private EmpleadoReglaHoraExtraDto? _actual;
    private bool _cargando;

    public bool HuboCambios { get; private set; }

    public ConfigurarHoraExtraEmpleadoWindow(
        EmpleadoDto empleado,
        int negocioId)
    {
        InitializeComponent();

        _empleado = empleado;
        _negocioId = negocioId;
        _horaExtraApi = new HoraExtraApiService();
        _reglas = new List<ReglaHoraExtraDto>();

        txtEmpleado.Text =
            $"{empleado.Codigo} - {empleado.Nombres} {empleado.Apellidos}";

        dpFechaVigencia.SelectedDate = DateTime.Today;

        Loaded += ConfigurarHoraExtraEmpleadoWindow_Loaded;
    }

    // =========================================================
    // CARGA
    // =========================================================

    private async void ConfigurarHoraExtraEmpleadoWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await CargarTodoAsync();
    }

    private async Task CargarTodoAsync()
    {
        try
        {
            _cargando = true;

            _reglas =
                (await _horaExtraApi.ObtenerReglasAsync(_negocioId))
                .Where(x => x.Estado)
                .OrderBy(x => x.Nombre)
                .ToList();

            cmbRegla.ItemsSource = _reglas;

            _actual =
                await _horaExtraApi
                    .ObtenerActualAsync(_empleado.ID);

            MostrarConfiguracionActual();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo cargar la configuración de horas extra.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _cargando = false;

            ActualizarDisponibilidad();
            ActualizarTarifaSeleccionada();
        }
    }

    private void MostrarConfiguracionActual()
    {
        if (_actual == null)
        {
            txtReglaActual.Text =
                "Sin configuración activa";

            txtTarifaActual.Text =
                "No aplica";

            txtFechaActual.Text =
                "--";

            chkAplicaHorasExtra.IsChecked =
                false;

            cmbRegla.SelectedItem =
                _reglas.FirstOrDefault();

            dpFechaVigencia.SelectedDate =
                DateTime.Today;

            return;
        }

        txtReglaActual.Text =
            _actual.ReglaNombre;

        txtTarifaActual.Text =
            $"Q {_actual.TarifaHora:N2}/h";

        txtFechaActual.Text =
            _actual.FechaInicio.ToString("dd/MM/yyyy");

        chkAplicaHorasExtra.IsChecked =
            true;

        cmbRegla.SelectedItem =
            _reglas.FirstOrDefault(x =>
                x.ID == _actual.ReglaHoraExtraID);

        var fechaSugerida =
            DateTime.Today > _actual.FechaInicio.Date
                ? DateTime.Today
                : _actual.FechaInicio.Date.AddDays(1);

        dpFechaVigencia.SelectedDate =
            fechaSugerida;
    }

    // =========================================================
    // UI
    // =========================================================

    private void ChkAplicaHorasExtra_Changed(
        object sender,
        RoutedEventArgs e)
    {
        if (_cargando)
            return;

        ActualizarDisponibilidad();
        ActualizarTarifaSeleccionada();
    }

    private void CmbRegla_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_cargando)
            return;

        ActualizarTarifaSeleccionada();
    }

    private void ActualizarDisponibilidad()
    {
        bool aplica =
            chkAplicaHorasExtra.IsChecked == true;

        cmbRegla.IsEnabled =
            aplica;

        txtEtiquetaFecha.Text =
            aplica
                ? "FECHA DE INICIO DE LA CONFIGURACIÓN"
                : "FECHA DESDE LA QUE DEJA DE APLICAR";

        if (aplica)
        {
            txtAviso.Text =
                "Cambiar una regla no elimina el historial. " +
                "La configuración anterior se cerrará el día anterior " +
                "a la nueva vigencia.";
        }
        else
        {
            txtAviso.Text =
                _actual == null
                    ? "El empleado no tiene habilitadas horas extra."
                    : "Al guardar, FerreControl cerrará la configuración actual. " +
                      "Desde la fecha indicada el empleado dejará de generar horas extra.";
        }
    }

    private void ActualizarTarifaSeleccionada()
    {
        if (chkAplicaHorasExtra.IsChecked != true)
        {
            txtTarifaSeleccionada.Text =
                "No aplica";

            return;
        }

        if (cmbRegla.SelectedItem
            is not ReglaHoraExtraDto regla)
        {
            txtTarifaSeleccionada.Text =
                "--";

            return;
        }

        txtTarifaSeleccionada.Text =
            $"Q {regla.TarifaHora:N2}";
    }

    // =========================================================
    // ADMINISTRAR REGLAS
    // =========================================================

    private async void BtnAdministrarReglas_Click(
        object sender,
        RoutedEventArgs e)
    {
        var ventana =
            new HorasExtraWindow
            {
                Owner = this
            };

        ventana.ShowDialog();

        await RecargarReglasAsync();
    }

    private async Task RecargarReglasAsync()
    {
        try
        {
            _cargando = true;

            int? reglaSeleccionadaId =
                (cmbRegla.SelectedItem as ReglaHoraExtraDto)?.ID;

            _reglas =
                (await _horaExtraApi.ObtenerReglasAsync(_negocioId))
                .Where(x => x.Estado)
                .OrderBy(x => x.Nombre)
                .ToList();

            cmbRegla.ItemsSource = _reglas;

            if (reglaSeleccionadaId.HasValue)
            {
                cmbRegla.SelectedItem =
                    _reglas.FirstOrDefault(x =>
                        x.ID == reglaSeleccionadaId.Value);
            }

            cmbRegla.SelectedItem ??=
                _reglas.FirstOrDefault();
        }
        finally
        {
            _cargando = false;
            ActualizarTarifaSeleccionada();
        }
    }

    // =========================================================
    // GUARDAR
    // =========================================================

    private async void BtnGuardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!dpFechaVigencia.SelectedDate.HasValue)
        {
            MostrarAdvertencia(
                "Seleccione la fecha de vigencia.");

            return;
        }

        var fecha =
            dpFechaVigencia.SelectedDate.Value.Date;

        bool aplica =
            chkAplicaHorasExtra.IsChecked == true;

        try
        {
            btnGuardar.IsEnabled = false;

            // =====================================================
            // DESACTIVAR
            // =====================================================

            if (!aplica)
            {
                if (_actual == null)
                {
                    MessageBox.Show(
                        "El empleado ya se encuentra sin configuración de horas extra.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    DialogResult = true;
                    return;
                }

                if (fecha <= _actual.FechaInicio.Date)
                {
                    MostrarAdvertencia(
                        "La fecha en que deja de aplicar debe ser posterior " +
                        "al inicio de la configuración actual.");

                    return;
                }

                var confirmar =
                    MessageBox.Show(
                        $"{_empleado.Nombres} {_empleado.Apellidos} dejará de generar " +
                        $"horas extra desde el {fecha:dd/MM/yyyy}.\n\n¿Desea continuar?",
                        "Desactivar horas extra",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                if (confirmar != MessageBoxResult.Yes)
                    return;

                await _horaExtraApi.DesactivarAsync(
                    _empleado.ID,
                    new DesactivarHoraExtraEmpleadoDto
                    {
                        FechaInicioNoAplica = fecha
                    });

                HuboCambios = true;

                MessageBox.Show(
                    "La configuración de horas extra fue desactivada correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                return;
            }

            // =====================================================
            // ASIGNAR / CAMBIAR REGLA
            // =====================================================

            if (cmbRegla.SelectedItem
                is not ReglaHoraExtraDto regla)
            {
                MostrarAdvertencia(
                    "Seleccione una regla de horas extra.");

                return;
            }

            if (_actual != null)
            {
                if (_actual.ReglaHoraExtraID == regla.ID)
                {
                    MessageBox.Show(
                        "El empleado ya tiene esta regla de horas extra asignada.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                if (fecha <= _actual.FechaInicio.Date)
                {
                    MostrarAdvertencia(
                        "La nueva configuración debe iniciar después " +
                        "de la configuración actual.");

                    return;
                }
            }

            await _horaExtraApi.AsignarAsync(
                _empleado.ID,
                new AsignarReglaHoraExtraDto
                {
                    ReglaHoraExtraID = regla.ID,
                    FechaInicio = fecha
                });

            HuboCambios = true;

            MessageBox.Show(
                $"Configuración guardada correctamente.\n\n" +
                $"Regla: {regla.Nombre}\n" +
                $"Tarifa: Q {regla.TarifaHora:N2}/h\n" +
                $"Desde: {fecha:dd/MM/yyyy}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo guardar la configuración.\n\n{ex.Message}",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            btnGuardar.IsEnabled = true;
        }
    }

    private static void MostrarAdvertencia(
        string mensaje)
    {
        MessageBox.Show(
            mensaje,
            "FerreControl",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private void BtnCancelar_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
