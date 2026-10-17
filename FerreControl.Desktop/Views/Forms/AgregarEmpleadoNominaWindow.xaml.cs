using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.Nomina;
using FerreControl.Desktop.Services;
using System.Windows;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class AgregarEmpleadoNominaWindow
        : Window
    {
        private readonly NominaApiService
            _nominaApi;

        private readonly EmpleadoApiService
            _empleadoApi;

        private readonly int
            _nominaId;

        private readonly int
            _negocioId;

        private readonly HashSet<int>
            _empleadosExistentes;


        public AgregarEmpleadoNominaWindow(
            int nominaId,
            int negocioId,
            IEnumerable<int> empleadosExistentes)
        {
            InitializeComponent();


            _nominaId =
                nominaId;

            _negocioId =
                negocioId;

            _empleadosExistentes =
                empleadosExistentes
                    .ToHashSet();


            _nominaApi =
                new NominaApiService();

            _empleadoApi =
                new EmpleadoApiService();


            Loaded +=
                AgregarEmpleadoNominaWindow_Loaded;
        }


        private async void
            AgregarEmpleadoNominaWindow_Loaded(
                object sender,
                RoutedEventArgs e)
        {
            try
            {
                var empleados =
                    await _empleadoApi
                        .GetObtenerTodos();


                var opciones =
                    empleados
                        .Where(x =>
                            x.NegocioID == _negocioId
                            &&
                            x.Estado
                            &&
                            !_empleadosExistentes.Contains(
                                x.ID))
                        .OrderBy(x =>
                            x.Nombres)
                        .ThenBy(x =>
                            x.Apellidos)
                        .Select(x =>
                            new EmpleadoOpcion
                            {
                                ID =
                                    x.ID,

                                Nombre =
                                    $"{x.Codigo} - {x.Nombres} {x.Apellidos}"
                            })
                        .ToList();


                cmbEmpleado.ItemsSource =
                    opciones;


                if (opciones.Any())
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
        }


        private async void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbEmpleado.SelectedItem
                is not EmpleadoOpcion empleado)
            {
                MessageBox.Show(
                    "Seleccione un empleado.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!decimal.TryParse(
                txtBonificaciones.Text,
                out decimal bonificaciones)
                ||
                bonificaciones < 0)
            {
                MostrarMontoInvalido(
                    "bonificaciones");

                return;
            }


            if (!decimal.TryParse(
                txtHorasExtra.Text,
                out decimal horasExtra)
                ||
                horasExtra < 0)
            {
                MostrarMontoInvalido(
                    "horas extra");

                return;
            }


            if (!decimal.TryParse(
                txtOtrosDescuentos.Text,
                out decimal otrosDescuentos)
                ||
                otrosDescuentos < 0)
            {
                MostrarMontoInvalido(
                    "otros descuentos");

                return;
            }


            try
            {
                btnGuardar.IsEnabled =
                    false;


                var dto =
                    new CrearNominaDetalleDto
                    {
                        EmpleadoID =
                            empleado.ID,

                        Bonificaciones =
                            bonificaciones,

                        HorasExtra =
                            horasExtra,

                        OtrosDescuentos =
                            otrosDescuentos
                    };


                await _nominaApi
                    .AgregarDetalleAsync(
                        _nominaId,
                        dto);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo agregar el empleado.\n\n{ex.Message}",
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


        private void MostrarMontoInvalido(
            string campo)
        {
            MessageBox.Show(
                $"Ingrese un monto válido para {campo}.",
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }


        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult =
                false;
        }


        private class EmpleadoOpcion
        {
            public int ID { get; set; }

            public string Nombre { get; set; } =
                string.Empty;
        }
    }
}