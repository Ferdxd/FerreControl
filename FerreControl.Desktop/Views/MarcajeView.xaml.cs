using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Desktop.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace FerreControl.Desktop.Views
{
    public partial class MarcajeView
        : UserControl
    {
        private readonly MarcacionAsistenciaApiService
            _marcacionApi;


        private readonly DispatcherTimer
            _reloj;


        private bool
            _procesando;


        public MarcajeView()
        {
            InitializeComponent();


            _marcacionApi =
                new MarcacionAsistenciaApiService();


            _reloj =
                new DispatcherTimer
                {
                    Interval =
                        TimeSpan.FromSeconds(1)
                };


            _reloj.Tick +=
                Reloj_Tick;


            cmbTipoCredencial.SelectedIndex =
                0;


            Loaded +=
                MarcajeView_Loaded;


            Unloaded +=
                MarcajeView_Unloaded;
        }


        // =====================================================
        // CARGA
        // =====================================================

        private void MarcajeView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            ActualizarReloj();


            _reloj.Start();


            txtCodigoMarcaje.Focus();
        }


        private void MarcajeView_Unloaded(
            object sender,
            RoutedEventArgs e)
        {
            _reloj.Stop();
        }


        // =====================================================
        // RELOJ
        // =====================================================

        private void Reloj_Tick(
            object? sender,
            EventArgs e)
        {
            ActualizarReloj();
        }


        private void ActualizarReloj()
        {
            var ahora =
                DateTime.Now;


            txtHoraMarcaje.Text =
                ahora.ToString(
                    "HH:mm:ss");


            txtFechaMarcaje.Text =
                ahora.ToString(
                    "dddd, dd 'de' MMMM 'de' yyyy",
                    new CultureInfo(
                        "es-GT"));
        }


        // =====================================================
        // LECTOR
        // =====================================================

        private async void TxtCodigoMarcaje_PreviewKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key !=
                Key.Enter)
            {
                return;
            }


            e.Handled =
                true;


            if (_procesando)
                return;


            await RegistrarMarcacionAsync();
        }


        private async Task RegistrarMarcacionAsync()
        {
            var codigo =
                txtCodigoMarcaje.Text
                    .Trim();


            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                txtEstadoMarcaje.Text =
                    "No se recibió ningún código.";


                txtCodigoMarcaje.Focus();

                return;
            }


            if (cmbTipoCredencial.SelectedItem
                is not ComboBoxItem item)
            {
                txtEstadoMarcaje.Text =
                    "Seleccione el tipo de credencial.";

                return;
            }


            string tipoCredencial =
                item.Tag?.ToString()
                ??
                string.Empty;


            try
            {
                _procesando =
                    true;


                txtCodigoMarcaje.IsEnabled =
                    false;


                cmbTipoCredencial.IsEnabled =
                    false;


                txtEstadoMarcaje.Text =
                    "Procesando marcación...";


                var dto =
                    new RegistrarMarcacionAutomaticaDto
                    {
                        TipoCredencial =
                            tipoCredencial,

                        Codigo =
                            codigo,

                        Origen =
                            "FERRECONTROL_DESKTOP"
                    };


                var resultado =
                    await _marcacionApi
                        .RegistrarAutomaticoAsync(
                            dto);


                MostrarResultado(
                    resultado);


                txtEstadoMarcaje.Text =
                    $"{FormatearTipo(resultado.TipoMarcacion)} registrada correctamente.";
            }
            catch (Exception ex)
            {
                txtEstadoMarcaje.Text =
                    $"No se pudo registrar: {ex.Message}";
            }
            finally
            {
                _procesando =
                    false;


                txtCodigoMarcaje.Text =
                    string.Empty;


                txtCodigoMarcaje.IsEnabled =
                    true;


                cmbTipoCredencial.IsEnabled =
                    true;


                txtCodigoMarcaje.Focus();
            }
        }


        // =====================================================
        // RESULTADO
        // =====================================================

        private void MostrarResultado(
            MarcacionAsistenciaDto marcacion)
        {
            txtNombreEmpleado.Text =
                marcacion.NombreEmpleado;


            txtCodigoEmpleado.Text =
                $"Código: {marcacion.CodigoEmpleado}";


            txtInicialesEmpleado.Text =
                ObtenerIniciales(
                    marcacion.NombreEmpleado);


            txtTipoMarcacion.Text =
                FormatearTipo(
                    marcacion.TipoMarcacion);


            txtMetodoMarcacion.Text =
                FormatearMetodo(
                    marcacion.MetodoMarcacion);


            txtHoraRegistrada.Text =
                marcacion.FechaHora
                    .ToString(
                        "HH:mm:ss");
        }


        private static string ObtenerIniciales(
            string nombre)
        {
            if (string.IsNullOrWhiteSpace(
                nombre))
            {
                return "--";
            }


            var partes =
                nombre.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);


            if (partes.Length == 1)
            {
                return partes[0]
                    .Substring(
                        0,
                        1)
                    .ToUpperInvariant();
            }


            return
                $"{partes[0][0]}{partes[1][0]}"
                    .ToUpperInvariant();
        }


        private static string FormatearTipo(
            string tipo)
        {
            return tipo switch
            {
                "ENTRADA" =>
                    "Entrada",

                "INICIO_DESCANSO" =>
                    "Salida a descanso",

                "FIN_DESCANSO" =>
                    "Regreso de descanso",

                "SALIDA" =>
                    "Salida final",

                _ =>
                    tipo
            };
        }


        private static string FormatearMetodo(
            string metodo)
        {
            return metodo switch
            {
                "CODIGO_BARRAS" =>
                    "Código de barras",

                "QR" =>
                    "QR",

                "RFID" =>
                    "RFID",

                "HUELLA" =>
                    "Huella",

                "MANUAL" =>
                    "Registro manual",

                _ =>
                    metodo
            };
        }
    }
}