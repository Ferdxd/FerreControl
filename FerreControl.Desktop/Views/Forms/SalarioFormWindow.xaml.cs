using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.Salarios;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class SalarioFormWindow : Window
    {
        private readonly SalariosApiService _salariosApi;

        private readonly List<EmpleadoDto> _empleados;

        private SalarioEmpleadoDto? _salarioActual;


        public SalarioFormWindow(
            SalariosApiService salariosApi,
            List<EmpleadoDto> empleados,
            int? empleadoSeleccionadoId = null)
        {
            InitializeComponent();

            _salariosApi =
                salariosApi;

            _empleados =
                empleados;


            cmbEmpleado.ItemsSource =
                _empleados;

            cmbPeriodicidad.SelectedIndex =
                0;

            dpFechaInicio.SelectedDate =
                DateTime.Today;


            if (empleadoSeleccionadoId.HasValue)
            {
                cmbEmpleado.SelectedItem =
                    _empleados.FirstOrDefault(
                        x => x.ID ==
                             empleadoSeleccionadoId.Value);

                cmbEmpleado.IsEnabled =
                    false;
            }
            else if (_empleados.Any())
            {
                cmbEmpleado.SelectedIndex =
                    0;
            }
        }


        // =========================================================
        // CAMBIAR EMPLEADO
        // =========================================================

        private async void CmbEmpleado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbEmpleado.SelectedItem
                is not EmpleadoDto empleado)
            {
                return;
            }


            try
            {
                _salarioActual =
                    await _salariosApi
                        .ObtenerActualAsync(
                            empleado.ID);


                if (_salarioActual == null)
                {
                    txtTitulo.Text =
                        "Registrar salario";

                    txtSubtitulo.Text =
                        "Registra el primer salario del empleado.";

                    panelSalarioActual.Visibility =
                        Visibility.Collapsed;

                    txtMonto.Text = "";

                    dpFechaInicio.SelectedDate =
                        DateTime.Today;

                    return;
                }


                txtTitulo.Text =
                    "Cambiar salario";

                txtSubtitulo.Text =
                    "El salario anterior quedará almacenado en el historial.";


                panelSalarioActual.Visibility =
                    Visibility.Visible;


                txtMontoActual.Text =
                    $"Q {_salarioActual.Monto:N2}";


                txtFechaActual.Text =
                    _salarioActual.FechaInicio
                        .ToString("dd/MM/yyyy");


                txtMonto.Text =
                    _salarioActual.Monto
                        .ToString("0.00");


                SeleccionarPeriodicidad(
                    _salarioActual.Periodicidad);


                dpFechaInicio.SelectedDate =
                    DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // GUARDAR
        // =========================================================

        private async void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (cmbEmpleado.SelectedItem
                    is not EmpleadoDto empleado)
                {
                    MostrarAdvertencia(
                        "Debe seleccionar un empleado.");

                    return;
                }


                if (!decimal.TryParse(
                    txtMonto.Text.Trim(),
                    out decimal monto)
                    || monto <= 0)
                {
                    MostrarAdvertencia(
                        "Ingrese un salario válido.");

                    return;
                }


                if (cmbPeriodicidad.SelectedItem
                    is not ComboBoxItem item)
                {
                    MostrarAdvertencia(
                        "Seleccione la periodicidad.");

                    return;
                }


                if (!dpFechaInicio.SelectedDate
                    .HasValue)
                {
                    MostrarAdvertencia(
                        "Seleccione la fecha de inicio.");

                    return;
                }


                var fechaInicio =
                    dpFechaInicio
                        .SelectedDate.Value.Date;


                if (_salarioActual != null &&
                    fechaInicio <=
                    _salarioActual.FechaInicio.Date)
                {
                    MostrarAdvertencia(
                        "La fecha del nuevo salario debe ser posterior al inicio del salario actual.");

                    return;
                }


                var dto =
                    new CrearSalarioEmpleadoDto
                    {
                        Monto =
                            monto,

                        Periodicidad =
                            item.Content.ToString()
                            ?? "",

                        FechaInicio =
                            fechaInicio
                    };


                btnGuardar.IsEnabled =
                    false;


                await _salariosApi
                    .RegistrarAsync(
                        empleado.ID,
                        dto);


                MessageBox.Show(
                    _salarioActual == null
                        ? "Salario registrado correctamente."
                        : "Salario actualizado correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult = true;

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
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


        private void SeleccionarPeriodicidad(
            string periodicidad)
        {
            foreach (var item in
                cmbPeriodicidad.Items)
            {
                if (item is ComboBoxItem combo &&
                    string.Equals(
                        combo.Content?.ToString(),
                        periodicidad,
                        StringComparison.OrdinalIgnoreCase))
                {
                    cmbPeriodicidad.SelectedItem =
                        combo;

                    break;
                }
            }
        }


        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
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
    }
}