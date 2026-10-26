using FerreControl.Application.DTOs.EmpleadoCredencial;
using FerreControl.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class CredencialesEmpleadoWindow
        : Window
    {
        private readonly CarnetApiService
            _api;


        private readonly int
            _empleadoId;


        public bool HuboCambios
        {
            get;
            private set;
        }


        public CredencialesEmpleadoWindow(
            int empleadoId,
            string nombreEmpleado)
        {
            InitializeComponent();


            _empleadoId =
                empleadoId;


            _api =
                new CarnetApiService();


            txtEmpleado.Text =
                nombreEmpleado;


            cmbTipo.SelectedIndex =
                0;


            Loaded +=
                CredencialesEmpleadoWindow_Loaded;
        }


        private async void
            CredencialesEmpleadoWindow_Loaded(
                object sender,
                RoutedEventArgs e)
        {
            await CargarAsync();
        }


        private async Task CargarAsync()
        {
            try
            {
                var credenciales =
                    await _api
                        .ObtenerCredencialesAsync(
                            _empleadoId);


                dgCredenciales.ItemsSource =
                    credenciales;


                btnCambiarEstado.IsEnabled =
                    false;
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // AGREGAR
        // =====================================================

        private async void BtnAgregar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbTipo.SelectedItem
                is not ComboBoxItem item)
            {
                return;
            }


            var tipo =
                item.Tag?
                    .ToString()
                ??
                string.Empty;


            var codigo =
                txtCodigo.Text
                    .Trim();


            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                MessageBox.Show(
                    "Ingrese el identificador de la credencial.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                var dto =
                    new CrearEmpleadoCredencialDto
                    {
                        TipoCredencial =
                            tipo,

                        Codigo =
                            codigo
                    };


                await _api
                    .CrearManualAsync(
                        _empleadoId,
                        dto);


                txtCodigo.Text =
                    string.Empty;


                HuboCambios =
                    true;


                await CargarAsync();
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        // =====================================================
        // SELECCIÓN
        // =====================================================

        private void
            DgCredenciales_SelectionChanged(
                object sender,
                SelectionChangedEventArgs e)
        {
            if (dgCredenciales.SelectedItem
                is not EmpleadoCredencialDto credencial)
            {
                btnCambiarEstado.IsEnabled =
                    false;

                return;
            }


            btnCambiarEstado.IsEnabled =
                true;


            btnCambiarEstado.Content =
                credencial.Estado
                    ?
                    "Desactivar"
                    :
                    "Activar";
        }


        // =====================================================
        // ESTADO
        // =====================================================

        private async void
            BtnCambiarEstado_Click(
                object sender,
                RoutedEventArgs e)
        {
            if (dgCredenciales.SelectedItem
                is not EmpleadoCredencialDto credencial)
            {
                return;
            }


            bool nuevoEstado =
                !credencial.Estado;


            try
            {
                await _api
                    .ActualizarEstadoAsync(
                        _empleadoId,
                        credencial.ID,
                        nuevoEstado);


                HuboCambios =
                    true;


                await CargarAsync();
            }
            catch (Exception ex)
            {
                MostrarError(
                    ex.Message);
            }
        }


        private void BtnCerrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }


        private static void MostrarError(
            string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "FerreControl",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}