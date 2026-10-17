using FerreControl.Application.DTOs.Empleados;
using FerreControl.Desktop.Services;
using System.Windows;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class EmpleadoFormWindow : Window
    {
        private readonly EmpleadoApiService _empleadoApiService;

        private readonly EmpleadoDto? _empleado;


        // =========================================================
        // CONSTRUCTOR CREAR
        // =========================================================

        public EmpleadoFormWindow(EmpleadoApiService empleadoApiService)
        {
            InitializeComponent();

            _empleadoApiService = empleadoApiService;

            txtTitulo.Text = "Nuevo empleado";

            txtSubtitulo.Text ="Registra un nuevo empleado.";

            chkEstado.Visibility =Visibility.Collapsed;
        }

        // =========================================================
        // CONSTRUCTOR EDITAR
        // =========================================================

        public EmpleadoFormWindow(EmpleadoApiService empleadoApiService,EmpleadoDto empleado)
        {
            InitializeComponent();

            _empleadoApiService = empleadoApiService;

            _empleado = empleado;


            txtTitulo.Text = "Editar empleado";

            txtSubtitulo.Text = $"Código: {empleado.Codigo}";


            txtNegocioID.Text =empleado.NegocioID.ToString();

            txtNegocioID.IsEnabled = false;

            txtCodigo.Text = empleado.Codigo;

            txtNombres.Text = empleado.Nombres;

            txtApellidos.Text = empleado.Apellidos;

            txtTelefono.Text = empleado.Telefono ?? "";

            txtPuesto.Text =empleado.Puesto ?? "";

            chkEstado.IsChecked =empleado.Estado;

            chkEstado.Visibility =Visibility.Visible;
        }


        // =========================================================
        // GUARDAR
        // =========================================================

        private async void BtnGuardar_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    MostrarAdvertencia("Debe ingresar el código del empleado.");

                    return;
                }


                if (string.IsNullOrWhiteSpace(txtNombres.Text))
                {
                    MostrarAdvertencia("Debe ingresar los nombres.");

                    return;
                }


                if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    MostrarAdvertencia("Debe ingresar los apellidos.");

                    return;
                }


                btnGuardar.IsEnabled =false;
                // =============================================
                // NUEVO
                // =============================================

                if (_empleado == null)
                {
                    if (!int.TryParse(txtNegocioID.Text,out int negocioId))
                    {
                        MostrarAdvertencia("El ID del negocio no es válido.");

                        return;
                    }


                    var dto =
                        new CrearEmpleadoDto
                        {
                            NegocioID = negocioId,

                            Codigo =txtCodigo.Text.Trim(),

                            Nombres =txtNombres.Text.Trim(),

                            Apellidos =txtApellidos.Text.Trim(),

                            Telefono =string.IsNullOrWhiteSpace(txtTelefono.Text) ? null: txtTelefono.Text.Trim(),

                            FechaIngreso=dpFechaIngreso.SelectedDate.Value,

                            Puesto =string.IsNullOrWhiteSpace(txtPuesto.Text)? null: txtPuesto.Text.Trim()
                        };


                    await _empleadoApiService.PostCrearEmpleado(dto);
                }

                // =============================================
                // EDITAR
                // =============================================

                else
                {
                    var dto =
                        new ActualizarEmpleadoDto
                        {
                            
                            Codigo =txtCodigo.Text.Trim(),

                            Nombres =txtNombres.Text.Trim(),

                            Apellidos =txtApellidos.Text.Trim(),

                            Telefono =string.IsNullOrWhiteSpace(txtTelefono.Text)? null: txtTelefono.Text.Trim(),

                            Puesto =string.IsNullOrWhiteSpace(txtPuesto.Text)? null: txtPuesto.Text.Trim(),

                            Estado =chkEstado.IsChecked?? false
                        };


                    await _empleadoApiService.ActualizarAsync(_empleado.ID,dto);
                }


                MessageBox.Show(
                    _empleado == null
                        ? "Empleado registrado correctamente."
                        : "Empleado actualizado correctamente.",
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


        // =========================================================
        // CANCELAR
        // =========================================================

        private void BtnCancelar_Click(object sender,RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }


        private void MostrarAdvertencia(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}