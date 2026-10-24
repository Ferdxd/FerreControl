using FerreControl.Application.DTOs.EmpleadoHorario;
using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.Horarios;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class AsignarHorarioEmpleadoWindow
        : Window
    {
        private readonly HorarioDto
            _horario;


        private readonly List<EmpleadoDto>
            _empleados;


        private readonly EmpleadoHorarioApiService
            _empleadoHorarioApi;


        private EmpleadoHorarioDto?
            _horarioActual;


        private EmpleadoDto?
            _empleadoSeleccionado;


        private bool
            _cargando;


        public bool HorarioAsignado
        {
            get;
            private set;
        }


        public AsignarHorarioEmpleadoWindow(
            HorarioDto horario,
            List<EmpleadoDto> empleados)
        {
            InitializeComponent();


            _horario =
                horario;


            _empleados =
                empleados
                    .Where(x =>
                        x.Estado
                        &&
                        x.NegocioID ==
                        horario.NegocioID)
                    .OrderBy(x =>
                        x.Nombres)
                    .ThenBy(x =>
                        x.Apellidos)
                    .ToList();


            _empleadoHorarioApi =
                new EmpleadoHorarioApiService();


            txtHorario.Text =
                horario.Nombre;


            cmbEmpleado.ItemsSource =
                _empleados;


            dpFechaInicio.SelectedDate =
                DateTime.Today;


            dpFechaInicio.DisplayDateEnd =
                DateTime.Today;


            if (_empleados.Count > 0)
            {
                cmbEmpleado.SelectedIndex =
                    0;
            }
        }


        // =====================================================
        // SELECCIÓN EMPLEADO
        // =====================================================

        private async void CmbEmpleado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_cargando)
                return;


            if (cmbEmpleado.SelectedItem
                is not EmpleadoDto empleado)
            {
                _empleadoSeleccionado =
                    null;


                _horarioActual =
                    null;


                txtHorarioActual.Text =
                    "Seleccione un empleado";


                txtFechaHorarioActual.Text =
                    string.Empty;


                btnAsignar.IsEnabled =
                    false;


                return;
            }


            _empleadoSeleccionado =
                empleado;


            await CargarHorarioActualAsync();
        }


        // =====================================================
        // HORARIO ACTUAL
        // =====================================================

        private async Task CargarHorarioActualAsync()
        {
            if (_empleadoSeleccionado == null)
                return;


            try
            {
                _cargando =
                    true;


                btnAsignar.IsEnabled =
                    false;


                txtHorarioActual.Text =
                    "Consultando...";


                txtFechaHorarioActual.Text =
                    string.Empty;


                _horarioActual =
                    await _empleadoHorarioApi
                        .ObtenerActualAsync(
                            _empleadoSeleccionado.ID);


                if (_horarioActual == null)
                {
                    txtHorarioActual.Text =
                        "Sin horario asignado";


                    txtFechaHorarioActual.Text =
                        string.Empty;


                    txtAviso.Text =
                        "Este empleado todavía no tiene horario. " +
                        "La fecha seleccionada será el inicio de su primera asignación.";


                    var fechaMin =
                        _empleadoSeleccionado.FechaIngreso?
                            .Date
                        ??
                        DateTime.Today;


                    if (fechaMin >
                        DateTime.Today)
                    {
                        fechaMin =
                            DateTime.Today;
                    }


                    dpFechaInicio.DisplayDateStart =
                        fechaMin;


                    dpFechaInicio.SelectedDate =
                        DateTime.Today;


                    btnAsignar.IsEnabled =
                        true;


                    return;
                }


                txtHorarioActual.Text =
                    _horarioActual.HorarioNombre;


                txtFechaHorarioActual.Text =
                    $"Desde {_horarioActual.FechaInicio:dd/MM/yyyy}";


                // =================================================
                // YA TIENE EL MISMO HORARIO
                // =================================================

                if (_horarioActual.HorarioID ==
                    _horario.ID)
                {
                    txtAviso.Text =
                        "El empleado ya tiene este horario asignado actualmente.";


                    dpFechaInicio.DisplayDateStart =
                        _horarioActual.FechaInicio.Date;


                    dpFechaInicio.SelectedDate =
                        DateTime.Today;


                    btnAsignar.IsEnabled =
                        false;


                    return;
                }


                // =================================================
                // CAMBIO DE HORARIO
                // =================================================

                var fechaMinima =
                    _horarioActual.FechaInicio
                        .Date
                        .AddDays(1);


                dpFechaInicio.DisplayDateStart =
                    fechaMinima;


                if (DateTime.Today >=
                    fechaMinima)
                {
                    dpFechaInicio.SelectedDate =
                        DateTime.Today;
                }
                else
                {
                    dpFechaInicio.SelectedDate =
                        fechaMinima;
                }


                txtAviso.Text =
                    $"El empleado actualmente tiene el horario " +
                    $"\"{_horarioActual.HorarioNombre}\". " +
                    $"Al guardar, ese horario finalizará el día anterior " +
                    $"al inicio del nuevo horario. El historial se conservará.";


                btnAsignar.IsEnabled =
                    true;
            }
            catch (Exception ex)
            {
                _horarioActual =
                    null;


                txtHorarioActual.Text =
                    "No se pudo consultar";


                txtFechaHorarioActual.Text =
                    string.Empty;


                MessageBox.Show(
                    $"No se pudo consultar el horario actual.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _cargando =
                    false;
            }
        }


        // =====================================================
        // ASIGNAR
        // =====================================================

        private async void BtnAsignar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_empleadoSeleccionado == null)
            {
                MessageBox.Show(
                    "Seleccione un empleado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!dpFechaInicio.SelectedDate
                .HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha de inicio.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var fechaInicio =
                dpFechaInicio
                    .SelectedDate
                    .Value.Date;


            if (fechaInicio >
                DateTime.Today)
            {
                MessageBox.Show(
                    "Por ahora la fecha de inicio no puede ser futura.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (_empleadoSeleccionado.FechaIngreso
                .HasValue
                &&
                fechaInicio <
                _empleadoSeleccionado
                    .FechaIngreso
                    .Value.Date)
            {
                MessageBox.Show(
                    "El horario no puede comenzar antes de la fecha de ingreso del empleado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (_horarioActual != null)
            {
                if (_horarioActual.HorarioID ==
                    _horario.ID)
                {
                    MessageBox.Show(
                        "El empleado ya tiene este horario asignado.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                if (fechaInicio <=
                    _horarioActual.FechaInicio.Date)
                {
                    MessageBox.Show(
                        "La fecha del nuevo horario debe ser posterior " +
                        "a la fecha de inicio del horario actual.",
                        "FerreControl",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }
            }


            string mensaje;


            if (_horarioActual == null)
            {
                mensaje =
                    $"Se asignará el horario \"{_horario.Nombre}\" a:\n\n" +
                    $"{_empleadoSeleccionado.Nombres} " +
                    $"{_empleadoSeleccionado.Apellidos}\n\n" +
                    $"Desde: {fechaInicio:dd/MM/yyyy}\n\n" +
                    $"¿Desea continuar?";
            }
            else
            {
                mensaje =
                    $"Se cambiará el horario del empleado:\n\n" +
                    $"{_empleadoSeleccionado.Nombres} " +
                    $"{_empleadoSeleccionado.Apellidos}\n\n" +
                    $"Horario actual: {_horarioActual.HorarioNombre}\n" +
                    $"Nuevo horario: {_horario.Nombre}\n" +
                    $"Desde: {fechaInicio:dd/MM/yyyy}\n\n" +
                    $"¿Desea continuar?";
            }


            var confirmar =
                MessageBox.Show(
                    mensaje,
                    "Asignar horario",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmar !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                btnAsignar.IsEnabled =
                    false;


                var dto =
                    new CrearEmpleadoHorarioDto
                    {
                        HorarioID =
                            _horario.ID,

                        FechaInicio =
                            fechaInicio
                    };


                await _empleadoHorarioApi
                    .AsignarAsync(
                        _empleadoSeleccionado.ID,
                        dto);


                HorarioAsignado =
                    true;


                MessageBox.Show(
                    $"Horario asignado correctamente.\n\n" +
                    $"Empleado: {_empleadoSeleccionado.Nombres} " +
                    $"{_empleadoSeleccionado.Apellidos}\n" +
                    $"Horario: {_horario.Nombre}\n" +
                    $"Desde: {fechaInicio:dd/MM/yyyy}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo asignar el horario.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                btnAsignar.IsEnabled =
                    true;
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