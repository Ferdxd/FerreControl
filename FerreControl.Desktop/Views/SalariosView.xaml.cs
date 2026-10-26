using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class SalariosView : UserControl
    {
        private readonly EmpleadoApiService _empleadoApi;
        private readonly SalariosApiService _salariosApi;
        private readonly HoraExtraApiService _horaExtraApi;

        private List<EmpleadoDto> _empleados;
        private List<SalarioEmpleadoFila> _filas;

        private const int NegocioID = 1;


        public SalariosView()
        {
            InitializeComponent();

            _empleadoApi =
                new EmpleadoApiService();

            _salariosApi =
                new SalariosApiService();

            _horaExtraApi =
                new HoraExtraApiService();

            _empleados =
                new List<EmpleadoDto>();

            _filas =
                new List<SalarioEmpleadoFila>();

            cmbPeriodicidad.SelectedIndex =
                0;

            cmbEstadoSalario.SelectedIndex =
                0;

            Loaded +=
                SalariosView_Loaded;
        }


        // =====================================================
        // CARGA
        // =====================================================

        private async void SalariosView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarSalariosAsync();
        }


        private async Task CargarSalariosAsync(
            int? empleadoSeleccionadoId = null)
        {
            try
            {
                _empleados =
                    (await _empleadoApi
                        .GetObtenerTodos())
                    .Where(x =>
                        x.NegocioID == NegocioID)
                    .ToList();


                _filas.Clear();


                foreach (var empleado
                    in _empleados)
                {
                    var salario =
                        await _salariosApi
                            .ObtenerActualAsync(
                                empleado.ID);


                    _filas.Add(
                        new SalarioEmpleadoFila
                        {
                            EmpleadoID =
                                empleado.ID,

                            CodigoEmpleado =
                                empleado.Codigo,

                            NombreEmpleado =
                                $"{empleado.Nombres} {empleado.Apellidos}",

                            Puesto =
                                empleado.Puesto,

                            Monto =
                                salario?.Monto,

                            Periodicidad =
                                salario?.Periodicidad
                                ??
                                string.Empty,

                            FechaInicio =
                                salario?.FechaInicio,

                            TieneSalario =
                                salario != null,

                            EmpleadoActivo =
                                empleado.Estado
                        });
                }


                ActualizarIndicadores();

                AplicarFiltros();


                if (empleadoSeleccionadoId.HasValue)
                {
                    dgSalarios.SelectedItem =
                        dgSalarios.Items
                            .Cast<SalarioEmpleadoFila>()
                            .FirstOrDefault(
                                x =>
                                    x.EmpleadoID ==
                                    empleadoSeleccionadoId.Value);
                }
                else
                {
                    LimpiarDetalle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los salarios.\n\n{ex.Message}",
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
            var conSalario =
                _filas
                    .Where(x =>
                        x.TieneSalario)
                    .ToList();


            txtEmpleadosConSalario.Text =
                conSalario.Count
                    .ToString();


            txtEmpleadosSinSalario.Text =
                _filas
                    .Count(x =>
                        !x.TieneSalario)
                    .ToString();


            txtSalarioPromedio.Text =
                conSalario.Any()
                    ?
                    $"Q {conSalario.Average(x => x.Monto ?? 0):N2}"
                    :
                    "Q 0.00";
        }


        // =====================================================
        // FILTROS
        // =====================================================

        private void AplicarFiltros()
        {
            IEnumerable<SalarioEmpleadoFila>
                resultado =
                    _filas;


            var texto =
                txtBuscarSalario.Text
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
                        x.NombreEmpleado.Contains(
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


            if (cmbPeriodicidad.SelectedIndex >
                0)
            {
                var periodicidad =
                    (
                        cmbPeriodicidad.SelectedItem
                        as ComboBoxItem
                    )?.Content?
                    .ToString();


                resultado =
                    resultado.Where(x =>
                        string.Equals(
                            x.Periodicidad,
                            periodicidad,
                            StringComparison.OrdinalIgnoreCase));
            }


            if (cmbEstadoSalario.SelectedIndex ==
                1)
            {
                resultado =
                    resultado.Where(x =>
                        x.TieneSalario);
            }
            else if (
                cmbEstadoSalario.SelectedIndex ==
                2)
            {
                resultado =
                    resultado.Where(x =>
                        !x.TieneSalario);
            }


            dgSalarios.ItemsSource =
                resultado.ToList();
        }


        private void TxtBuscarSalario_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        private void CmbFiltros_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        // =====================================================
        // SELECCIÓN EMPLEADO
        // =====================================================

        private async void DgSalarios_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgSalarios.SelectedItem
                is not SalarioEmpleadoFila fila)
            {
                LimpiarDetalle();

                return;
            }


            txtEmpleadoSalario.Text =
                fila.NombreEmpleado;


            if (fila.TieneSalario)
            {
                txtSalarioActual.Text =
                    $"Q {fila.Monto:N2}";


                txtPeriodicidadActual.Text =
                    fila.Periodicidad;



                txtFechaInicioSalario.Text =
                    fila.FechaInicio?
                        .ToString(
                            "dd/MM/yyyy")
                    ??
                    "--";


                btnCambiarSalario.Content =
                    "Cambiar salario";
            }
            else
            {
                txtSalarioActual.Text =
                    "Sin salario";


                txtPeriodicidadActual.Text =
                    "--";


                txtFechaInicioSalario.Text =
                    "--";


                btnCambiarSalario.Content =
                    "Registrar salario";
            }


            btnCambiarSalario.IsEnabled =
                fila.EmpleadoActivo;


            btnConfigurarHoraExtra.IsEnabled =
                fila.EmpleadoActivo;


            await Task.WhenAll(
                CargarHistorialAsync(
                    fila.EmpleadoID),

                CargarHoraExtraActualAsync(
                    fila.EmpleadoID));
        }


        // =====================================================
        // HISTORIAL SALARIOS
        // =====================================================

        private async Task CargarHistorialAsync(
            int empleadoId)
        {
            try
            {
                var historial =
                    await _salariosApi
                        .ObtenerHistorialAsync(
                            empleadoId);


                dgHistorialSalarios.ItemsSource =
                    historial
                        .OrderByDescending(
                            x =>
                                x.FechaInicio)
                        .ToList();
            }
            catch
            {
                dgHistorialSalarios.ItemsSource =
                    null;
            }
        }


        // =====================================================
        // CONFIGURACIÓN ACTUAL HORAS EXTRA
        // =====================================================

        private async Task CargarHoraExtraActualAsync(
            int empleadoId)
        {
            try
            {
                var configuracion =
                    await _horaExtraApi
                        .ObtenerActualAsync(
                            empleadoId);


                MostrarHoraExtraActual(
                    configuracion);
            }
            catch (Exception ex)
            {
                txtEstadoHoraExtra.Text =
                    "ERROR";


                txtEstadoHoraExtra.Foreground =
                    FindResource(
                        "DangerBrush")
                    as System.Windows.Media.Brush;


                txtReglaHoraExtra.Text =
                    "No se pudo consultar";


                txtTarifaHoraExtra.Text =
                    "--";


                txtFechaHoraExtra.Text =
                    "--";


                System.Diagnostics.Debug.WriteLine(
                    $"Horas extra: {ex}");
            }
        }


        private void MostrarHoraExtraActual(
            EmpleadoReglaHoraExtraDto?
                configuracion)
        {
            if (configuracion == null)
            {
                txtEstadoHoraExtra.Text =
                    "NO HABILITADO";


                txtEstadoHoraExtra.Foreground =
                    FindResource(
                        "TextSecondaryBrush")
                    as System.Windows.Media.Brush;


                txtReglaHoraExtra.Text =
                    "Sin configuración activa";


                txtTarifaHoraExtra.Text =
                    "--";


                txtFechaHoraExtra.Text =
                    "--";


                return;
            }


            txtEstadoHoraExtra.Text =
                "HABILITADO";


            txtEstadoHoraExtra.Foreground =
                FindResource(
                    "SuccessBrush")
                as System.Windows.Media.Brush;


            txtReglaHoraExtra.Text =
                configuracion.ReglaNombre;


            txtTarifaHoraExtra.Text =
                $"Q {configuracion.TarifaHora:N2}/h";


            txtFechaHoraExtra.Text =
                configuracion.FechaInicio
                    .ToString(
                        "dd/MM/yyyy");
        }


        // =====================================================
        // REGISTRAR SALARIO
        // =====================================================

        private async void BtnRegistrarSalario_Click(
            object sender,
            RoutedEventArgs e)
        {
            var empleadosActivos =
                _empleados
                    .Where(x =>
                        x.Estado)
                    .ToList();


            if (empleadosActivos.Count ==
                0)
            {
                MessageBox.Show(
                    "No existen empleados activos.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            var ventana =
                new SalarioFormWindow(
                    _salariosApi,
                    empleadosActivos)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() ==
                true)
            {
                await CargarSalariosAsync();
            }
        }


        // =====================================================
        // CAMBIAR SALARIO
        // =====================================================

        private async void BtnCambiarSalario_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgSalarios.SelectedItem
                is not SalarioEmpleadoFila fila)
            {
                MessageBox.Show(
                    "Seleccione un empleado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            var empleado =
                _empleados
                    .FirstOrDefault(
                        x =>
                            x.ID ==
                            fila.EmpleadoID);


            if (empleado == null)
            {
                MessageBox.Show(
                    "No se encontró el empleado seleccionado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            var ventana =
                new SalarioFormWindow(
                    _salariosApi,
                    new List<EmpleadoDto>
                    {
                        empleado
                    },
                    empleado.ID)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() ==
                true)
            {
                await CargarSalariosAsync(
                    empleado.ID);
            }
        }


        // =====================================================
        // CONFIGURAR HORAS EXTRA
        // =====================================================

        private async void BtnConfigurarHoraExtra_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                /*
                 * No dependemos únicamente de SelectedItem.
                 * También comprobamos que exista un empleado
                 * realmente cargado.
                 */

                if (dgSalarios.SelectedItem
                    is not SalarioEmpleadoFila fila)
                {
                    MessageBox.Show(
                        "Seleccione primero un empleado.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                var empleado =
                    _empleados
                        .FirstOrDefault(
                            x =>
                                x.ID ==
                                fila.EmpleadoID);


                if (empleado == null)
                {
                    MessageBox.Show(
                        "No se encontró el empleado seleccionado.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }


                if (!empleado.Estado)
                {
                    MessageBox.Show(
                        "El empleado está inactivo y no puede configurarse para horas extra.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                /*
                 * Construimos la ventana dentro del try.
                 * Si InitializeComponent() o cualquier recurso
                 * XAML falla, veremos el error real.
                 */

                var ventana =
                    new ConfigurarHoraExtraEmpleadoWindow(
                        empleado,
                        empleado.NegocioID);


                var owner =
                    Window.GetWindow(this);


                if (owner != null)
                {
                    ventana.Owner =
                        owner;
                }


                ventana.WindowStartupLocation =
                    owner != null
                        ?
                        WindowStartupLocation.CenterOwner
                        :
                        WindowStartupLocation.CenterScreen;


                ventana.ShowActivated =
                    true;


                /*
                 * ShowDialog mantiene Salarios detrás y evita
                 * que se abra más de una configuración a la vez.
                 */

                var resultado =
                    ventana.ShowDialog();


                if (resultado == true
                    &&
                    ventana.HuboCambios)
                {
                    await CargarHoraExtraActualAsync(
                        empleado.ID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo abrir la configuración de horas extra.\n\n" +
                    ex.Message +
                    "\n\n" +
                    "Tipo: " +
                    ex.GetType().Name,
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // LIMPIAR
        // =====================================================

        private void LimpiarDetalle()
        {
            txtEmpleadoSalario.Text =
                "Selecciona un empleado";


            txtSalarioActual.Text =
                "--";


            txtPeriodicidadActual.Text =
                "--";


            txtFechaInicioSalario.Text =
                "--";


            btnCambiarSalario.Content =
                "Registrar salario";


            btnCambiarSalario.IsEnabled =
                false;


            btnConfigurarHoraExtra.IsEnabled =
                false;


            txtEstadoHoraExtra.Text =
                "--";


            txtEstadoHoraExtra.Foreground =
                FindResource(
                    "TextSecondaryBrush")
                as System.Windows.Media.Brush;


            txtReglaHoraExtra.Text =
                "Sin configuración";


            txtTarifaHoraExtra.Text =
                "--";


            txtFechaHoraExtra.Text =
                "--";


            dgHistorialSalarios.ItemsSource =
                null;
        }
    }
}