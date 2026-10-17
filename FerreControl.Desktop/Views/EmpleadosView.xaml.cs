using FerreControl.Application.DTOs.Empleados;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FerreControl.Desktop.Views
{
    public partial class EmpleadosView : UserControl
    {
        private readonly EmpleadoApiService _empleadoApiService;
        private List<EmpleadoDto> _empleados;
        public EmpleadosView()
        {
            InitializeComponent();
            _empleadoApiService = new EmpleadoApiService();
            _empleados = new List<EmpleadoDto>();
            cmbEstadoEmpleado.SelectedIndex = 0;
            Loaded += EmpleadosView_Loaded;
        }

        private async void EmpleadosView_Loaded(object sender, RoutedEventArgs e)
        {
            await cargarEmpleadosAsync();
        }

        private async Task cargarEmpleadosAsync()
        {
            try
            {
                btnNuevoEmpleado.IsEnabled = false;

                _empleados =
                    await _empleadoApiService
                        .GetObtenerTodos();

                aplicarFiltros();

                ActualizarIndicadores();
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
                btnNuevoEmpleado.IsEnabled = true;
            }
        }

        private void aplicarFiltros()
        {
            IEnumerable<EmpleadoDto> resultado = _empleados;
            var texto = txtBuscarEmpleado.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(texto))
            {
                resultado = resultado.Where(x => x.Codigo.ToLower().Contains(texto) || x.Nombres.ToLower().Contains(texto)
                ||x.Apellidos.ToLower().Contains(texto)||(x.Puesto??"").ToLower().Contains(texto));

            }
            if (cmbEstadoEmpleado.SelectedIndex == 1)
            {
                resultado = resultado.Where(x => x.Estado);
            }
            if (cmbEstadoEmpleado.SelectedIndex == 2)
            {
                resultado = resultado.Where(x => !x.Estado);
            }
            dgEmpleados.ItemsSource = resultado.ToList();
        }

        private void ActualizarIndicadores()
        {
            txtTotalEmpleados.Text =_empleados.Count.ToString();

            txtEmpleadosActivos.Text =_empleados
                    .Count(x => x.Estado)
                    .ToString();

            txtEmpleadosInactivos.Text =_empleados
                    .Count(x => !x.Estado)
                    .ToString();
        }


        private void TxtBuscarEmpleado_TextChanged(object sender,TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            aplicarFiltros();
        }


        private void CmbEstadoEmpleado_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            aplicarFiltros();
        }
        private async void BtnNuevoEmpleado_Click(object sender,RoutedEventArgs e)
        {
            var ventana =new EmpleadoFormWindow(_empleadoApiService);

            ventana.Owner =Window.GetWindow(this);

            var resultado =ventana.ShowDialog();


            if (resultado == true)
            {
                await cargarEmpleadosAsync();
            }
        }


        private async void DgEmpleados_MouseDoubleClick(object sender,MouseButtonEventArgs e)
        {
            if (dgEmpleados.SelectedItem
                is not EmpleadoDto empleado)
            {
                return;
            }


            var empleadoCompleto =await _empleadoApiService.ObtenerPorId(empleado.ID);


            if (empleadoCompleto == null)
            {
                MessageBox.Show(
                    "El empleado ya no existe.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                await cargarEmpleadosAsync();

                return;
            }


            var ventana =new EmpleadoFormWindow(_empleadoApiService,empleadoCompleto);

            ventana.Owner =Window.GetWindow(this);


            var resultado =ventana.ShowDialog();


            if (resultado == true)
            {
                await cargarEmpleadosAsync();
            }
        }

    }
}