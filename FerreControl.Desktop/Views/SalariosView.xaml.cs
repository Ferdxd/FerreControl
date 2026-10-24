using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Application.DTOs.Salarios;
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

            cmbPeriodicidad.SelectedIndex = 0;
            cmbEstadoSalario.SelectedIndex = 0;

            Loaded += SalariosView_Loaded;
        }


        private async void SalariosView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CargarSalariosAsync();
        }


        // =========================================================
        // CARGAR
        // =========================================================

        private async Task CargarSalariosAsync()
        {
            try
            {
                _empleados =
                    (await _empleadoApi.GetObtenerTodos())
                    .Where(x => x.NegocioID == NegocioID)
                    .ToList();

                _filas.Clear();

                foreach (var empleado in _empleados)
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
                                salario?.Periodicidad ?? "",

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
                LimpiarDetalle();
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


        // =========================================================
        // INDICADORES
        // =========================================================

        private void ActualizarIndicadores()
        {
            var conSalario =
                _filas.Where(x =>
                    x.TieneSalario).ToList();

            txtEmpleadosConSalario.Text =
                conSalario.Count.ToString();

            txtEmpleadosSinSalario.Text =
                _filas.Count(x =>
                    !x.TieneSalario)
                .ToString();

            txtSalarioPromedio.Text =
                conSalario.Any()
                    ? $"Q {conSalario.Average(x => x.Monto ?? 0):N2}"
                    : "Q 0.00";
        }


        // =========================================================
        // FILTROS
        // =========================================================

        private void AplicarFiltros()
        {
            IEnumerable<SalarioEmpleadoFila> resultado =
                _filas;

            var texto =
                txtBuscarSalario.Text
                    .Trim()
                    .ToLower();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                resultado =
                    resultado.Where(x =>
                        x.CodigoEmpleado
                            .ToLower()
                            .Contains(texto)

                        ||

                        x.NombreEmpleado
                            .ToLower()
                            .Contains(texto)

                        ||

                        (x.Puesto ?? "")
                            .ToLower()
                            .Contains(texto));
            }

            if (cmbPeriodicidad.SelectedIndex > 0)
            {
                var periodicidad =
                    ((ComboBoxItem)
                        cmbPeriodicidad.SelectedItem)
                    .Content.ToString();

                resultado =
                    resultado.Where(x =>
                        string.Equals(
                            x.Periodicidad,
                            periodicidad,
                            StringComparison.OrdinalIgnoreCase));
            }

            if (cmbEstadoSalario.SelectedIndex == 1)
            {
                resultado =
                    resultado.Where(x =>
                        x.TieneSalario);
            }
            else if (
                cmbEstadoSalario.SelectedIndex == 2)
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


        // =========================================================
        // SELECCIÓN
        // =========================================================

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
                        .ToString("dd/MM/yyyy")
                    ?? "--";

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


        // =========================================================
        // HISTORIAL SALARIO
        // =========================================================

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
                            x => x.FechaInicio)
                        .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cargar el historial.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CONFIGURACIÓN ACTUAL DE HORAS EXTRA
        // =========================================================

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
                    "Error";

                txtEstadoHoraExtra.Foreground =
                    FindResource("DangerBrush")
                    as System.Windows.Media.Brush;

                txtReglaHoraExtra.Text =
                    "No se pudo consultar";

                txtTarifaHoraExtra.Text =
                    "--";

                txtFechaHoraExtra.Text =
                    "--";

                MessageBox.Show(
                    $"No se pudo consultar la configuración de horas extra.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void MostrarHoraExtraActual(
            EmpleadoReglaHoraExtraDto? configuracion)
        {
            if (configuracion == null)
            {
                txtEstadoHoraExtra.Text =
                    "NO HABILITADO";

                txtEstadoHoraExtra.Foreground =
                    FindResource("TextSecondaryBrush")
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
                FindResource("SuccessBrush")
                as System.Windows.Media.Brush;

            txtReglaHoraExtra.Text =
                configuracion.ReglaNombre;

            txtTarifaHoraExtra.Text =
                $"Q {configuracion.TarifaHora:N2}/h";

            txtFechaHoraExtra.Text =
                configuracion.FechaInicio
                    .ToString("dd/MM/yyyy");
        }


        // =========================================================
        // REGISTRAR DESDE CABECERA
        // =========================================================

        private async void BtnRegistrarSalario_Click(
            object sender,
            RoutedEventArgs e)
        {
            var empleadosActivos =
                _empleados
                    .Where(x => x.Estado)
                    .ToList();

            var ventana =
                new SalarioFormWindow(
                    _salariosApi,
                    empleadosActivos);

            ventana.Owner =
                Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                await CargarSalariosAsync();
            }
        }


        // =========================================================
        // REGISTRAR / CAMBIAR SALARIO
        // =========================================================

        private async void BtnCambiarSalario_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgSalarios.SelectedItem
                is not SalarioEmpleadoFila fila)
            {
                return;
            }

            var empleado =
                _empleados.First(
                    x => x.ID ==
                         fila.EmpleadoID);

            var ventana =
                new SalarioFormWindow(
                    _salariosApi,
                    new List<EmpleadoDto>
                    {
                        empleado
                    },
                    empleado.ID);

            ventana.Owner =
                Window.GetWindow(this);

            if (ventana.ShowDialog() == true)
            {
                await CargarSalariosAsync();
            }
        }


        // =========================================================
        // CONFIGURAR HORAS EXTRA
        // =========================================================

        private async void BtnConfigurarHoraExtra_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgSalarios.SelectedItem
                is not SalarioEmpleadoFila fila)
            {
                return;
            }

            var empleado =
                _empleados.FirstOrDefault(x =>
                    x.ID == fila.EmpleadoID);

            if (empleado == null)
                return;

            var ventana =
                new ConfigurarHoraExtraEmpleadoWindow(
                    empleado,
                    NegocioID)
                {
                    Owner = Window.GetWindow(this)
                };

            if (ventana.ShowDialog() == true
                && ventana.HuboCambios)
            {
                await CargarHoraExtraActualAsync(
                    empleado.ID);
            }
        }


        // =========================================================
        // LIMPIAR
        // =========================================================

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
                FindResource("TextSecondaryBrush")
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
