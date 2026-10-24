using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Desktop.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class AsistenciaManualWindow
        : Window
    {
        private readonly int
            _negocioId;


        private readonly int?
            _empleadoInicialId;


        private readonly bool
            _soloConsulta;


        private readonly EmpleadoApiService
            _empleadoApi;


        private readonly MarcacionAsistenciaApiService
            _marcacionApi;


        private List<EmpleadoDto>
            _empleados;


        private List<MarcacionAsistenciaDto>
            _marcacionesExistentes;


        private bool
            _cargando;


        public bool HuboCambios
        {
            get;
            private set;
        }


        public AsistenciaManualWindow(
            int negocioId,
            DateTime fecha,
            int? empleadoId = null,
            bool soloConsulta = false)
        {
            InitializeComponent();


            _negocioId =
                negocioId;


            _empleadoInicialId =
                empleadoId;


            _soloConsulta =
                soloConsulta;


            _empleadoApi =
                new EmpleadoApiService();


            _marcacionApi =
                new MarcacionAsistenciaApiService();


            _empleados =
                new List<EmpleadoDto>();


            _marcacionesExistentes =
                new List<MarcacionAsistenciaDto>();


            dpFecha.SelectedDate =
                fecha.Date;


            if (_soloConsulta)
            {
                txtTitulo.Text =
                    "Marcaciones del empleado";


                txtModo.Text =
                    "Consulta de las marcaciones registradas para la fecha seleccionada.";


                btnGuardar.Visibility =
                    Visibility.Collapsed;


                txtObservacion.IsEnabled =
                    false;
            }


            Loaded +=
                AsistenciaManualWindow_Loaded;
        }


        // =====================================================
        // CARGA
        // =====================================================

        private async void AsistenciaManualWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarEmpleadosAsync();
        }


        private async Task CargarEmpleadosAsync()
        {
            try
            {
                _cargando =
                    true;


                _empleados =
                    (await _empleadoApi
                        .GetObtenerTodos())
                    .Where(x =>
                        x.NegocioID ==
                        _negocioId)
                    .OrderBy(x =>
                        x.Nombres)
                    .ThenBy(x =>
                        x.Apellidos)
                    .ToList();


                AplicarEmpleadosPorFecha();


                if (_empleadoInicialId.HasValue)
                {
                    cmbEmpleado.SelectedItem =
                        cmbEmpleado.Items
                            .Cast<EmpleadoDto>()
                            .FirstOrDefault(x =>
                                x.ID ==
                                _empleadoInicialId.Value);
                }
                else if (cmbEmpleado.Items.Count > 0)
                {
                    cmbEmpleado.SelectedIndex =
                        0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los empleados.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _cargando =
                    false;
            }


            await CargarMarcacionesActualesAsync();
        }


        private void AplicarEmpleadosPorFecha()
        {
            var fecha =
                dpFecha.SelectedDate
                ??
                DateTime.Today;


            var disponibles =
                _empleados
                    .Where(x =>
                        (
                            !x.FechaIngreso.HasValue
                            ||
                            x.FechaIngreso.Value.Date <=
                            fecha.Date
                        )
                        &&
                        (
                            !x.FechaBaja.HasValue
                            ||
                            x.FechaBaja.Value.Date >=
                            fecha.Date
                        ))
                    .ToList();


            var seleccionado =
                cmbEmpleado.SelectedItem
                as EmpleadoDto;


            cmbEmpleado.ItemsSource =
                disponibles;


            if (seleccionado != null)
            {
                cmbEmpleado.SelectedItem =
                    disponibles.FirstOrDefault(x =>
                        x.ID ==
                        seleccionado.ID);
            }
        }


        // =====================================================
        // CAMBIO EMPLEADO / FECHA
        // =====================================================

        private async void CmbEmpleado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_cargando)
                return;


            await CargarMarcacionesActualesAsync();
        }


        private async void DpFecha_SelectedDateChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded
                ||
                _cargando)
            {
                return;
            }


            _cargando =
                true;


            AplicarEmpleadosPorFecha();


            _cargando =
                false;


            await CargarMarcacionesActualesAsync();
        }


        // =====================================================
        // MARCACIONES EXISTENTES
        // =====================================================

        private async Task CargarMarcacionesActualesAsync()
        {
            LimpiarCampos();


            if (cmbEmpleado.SelectedItem
                is not EmpleadoDto empleado)
            {
                return;
            }


            if (!dpFecha.SelectedDate.HasValue)
                return;


            try
            {
                _marcacionesExistentes =
                    await _marcacionApi
                        .ObtenerPorEmpleadoYFechaAsync(
                            empleado.ID,
                            dpFecha.SelectedDate
                                .Value.Date);


                ConfigurarCampo(
                    txtEntrada,
                    "ENTRADA");


                ConfigurarCampo(
                    txtInicioDescanso,
                    "INICIO_DESCANSO");


                ConfigurarCampo(
                    txtFinDescanso,
                    "FIN_DESCANSO");


                ConfigurarCampo(
                    txtSalida,
                    "SALIDA");


                if (_soloConsulta)
                {
                    cmbEmpleado.IsEnabled =
                        false;


                    dpFecha.IsEnabled =
                        false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar las marcaciones.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void ConfigurarCampo(
            TextBox campo,
            string tipo)
        {
            var marcacion =
                _marcacionesExistentes
                    .FirstOrDefault(x =>
                        string.Equals(
                            x.TipoMarcacion,
                            tipo,
                            StringComparison.OrdinalIgnoreCase));


            if (marcacion != null)
            {
                campo.Text =
                    marcacion.FechaHora
                        .ToString(
                            "HH:mm");


                campo.IsEnabled =
                    false;
            }
            else
            {
                campo.Text =
                    string.Empty;


                campo.IsEnabled =
                    !_soloConsulta;
            }
        }


        private void LimpiarCampos()
        {
            txtEntrada.Text =
                string.Empty;


            txtInicioDescanso.Text =
                string.Empty;


            txtFinDescanso.Text =
                string.Empty;


            txtSalida.Text =
                string.Empty;


            txtEntrada.IsEnabled =
                !_soloConsulta;


            txtInicioDescanso.IsEnabled =
                !_soloConsulta;


            txtFinDescanso.IsEnabled =
                !_soloConsulta;


            txtSalida.IsEnabled =
                !_soloConsulta;
        }


        // =====================================================
        // GUARDAR
        // =====================================================

        private async void BtnGuardar_Click(
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


            if (!dpFecha.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione una fecha.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (dpFecha.SelectedDate.Value.Date >
                DateTime.Today)
            {
                MessageBox.Show(
                    "No puede registrar asistencia en una fecha futura.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                TimeOnly? entrada =
                    ObtenerHoraNueva(
                        txtEntrada,
                        "entrada");


                TimeOnly? inicioDescanso =
                    ObtenerHoraNueva(
                        txtInicioDescanso,
                        "salida a descanso");


                TimeOnly? finDescanso =
                    ObtenerHoraNueva(
                        txtFinDescanso,
                        "regreso de descanso");


                TimeOnly? salida =
                    ObtenerHoraNueva(
                        txtSalida,
                        "salida final");


                if (!entrada.HasValue
                    &&
                    !inicioDescanso.HasValue
                    &&
                    !finDescanso.HasValue
                    &&
                    !salida.HasValue)
                {
                    MessageBox.Show(
                        "No hay nuevas marcaciones para guardar.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                var dto =
                    new RegistrarAsistenciaManualDto
                    {
                        EmpleadoID =
                            empleado.ID,

                        Fecha =
                            dpFecha.SelectedDate
                                .Value.Date,

                        HoraEntrada =
                            entrada,

                        HoraInicioDescanso =
                            inicioDescanso,

                        HoraFinDescanso =
                            finDescanso,

                        HoraSalida =
                            salida,

                        Observacion =
                            string.IsNullOrWhiteSpace(
                                txtObservacion.Text)
                            ?
                            null
                            :
                            txtObservacion.Text
                                .Trim()
                    };


                btnGuardar.IsEnabled =
                    false;


                var resultado =
                    await _marcacionApi
                        .RegistrarManualAsync(
                            dto);


                HuboCambios =
                    resultado.Count > 0;


                MessageBox.Show(
                    $"Se registraron {resultado.Count} marcación(es) correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo registrar la asistencia.\n\n{ex.Message}",
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


        private TimeOnly? ObtenerHoraNueva(
            TextBox campo,
            string descripcion)
        {
            // El campo está bloqueado porque ya existe
            // una marcación de ese tipo.
            if (!campo.IsEnabled)
            {
                return null;
            }


            var texto =
                campo.Text
                    .Trim();


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                return null;
            }


            if (!TimeOnly.TryParseExact(
                texto,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out TimeOnly hora))
            {
                throw new FormatException(
                    $"La hora de {descripcion} no es válida.\n\n" +
                    "Utilice el formato HH:mm. Ejemplo: 07:30.");
            }


            return hora;
        }


        // =====================================================
        // CERRAR
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