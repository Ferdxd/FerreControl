using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Desktop.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FerreControl.Desktop.Views
{
    public partial class MarcajeView
        : UserControl
    {
        // =====================================================
        // CONFIGURACIÓN
        // =====================================================

        private const string TipoCredencialMarcaje =
            "CODIGO_BARRAS";


        /*
         * Evita que el lector registre dos marcaciones
         * distintas cuando envía el mismo código dos veces
         * accidentalmente.
         */
        private static readonly TimeSpan
            TiempoProteccionDobleLectura =
                TimeSpan.FromSeconds(10);


        // =====================================================
        // SERVICIOS
        // =====================================================

        private readonly MarcacionAsistenciaApiService
            _marcacionApi;


        // =====================================================
        // RELOJ
        // =====================================================

        private readonly DispatcherTimer
            _reloj;


        // =====================================================
        // ESTADO
        // =====================================================

        private bool
            _procesando;


        private readonly Dictionary<string, DateTime>
            _ultimasLecturasExitosas;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public MarcajeView()
        {
            InitializeComponent();


            _marcacionApi =
                new MarcacionAsistenciaApiService();


            _ultimasLecturasExitosas =
                new Dictionary<string, DateTime>(
                    StringComparer.Ordinal);


            _reloj =
                new DispatcherTimer
                {
                    Interval =
                        TimeSpan.FromSeconds(1)
                };


            _reloj.Tick +=
                Reloj_Tick;


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


            MostrarEstadoEsperando();


            Dispatcher.BeginInvoke(
                () =>
                {
                    txtCodigoMarcaje.Focus();

                    Keyboard.Focus(
                        txtCodigoMarcaje);
                },
                DispatcherPriority.Input);
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
        // LECTOR - ENTER
        // =====================================================

        private async void
            TxtCodigoMarcaje_PreviewKeyDown(
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


            await RegistrarMarcacionAsync();
        }


        // =====================================================
        // BOTÓN PARA PRUEBAS
        // =====================================================

        private async void BtnRegistrarCodigo_Click(
            object sender,
            RoutedEventArgs e)
        {
            await RegistrarMarcacionAsync();
        }


        // =====================================================
        // REGISTRAR MARCACIÓN
        // =====================================================

        private async Task RegistrarMarcacionAsync()
        {
            if (_procesando)
                return;


            var codigo =
                txtCodigoMarcaje.Text
                    .Trim();


            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                MostrarEstadoError(
                    "No se recibió ningún código de credencial.");


                RecuperarFoco();

                return;
            }


            // =================================================
            // PROTECCIÓN CONTRA DOBLE ESCANEO
            // =================================================

            if (EsLecturaDuplicada(
                codigo))
            {
                MostrarEstadoAdvertencia(
                    "La credencial acaba de ser leída. " +
                    "Espere unos segundos antes de volver a escanearla.");


                txtCodigoMarcaje.Text =
                    string.Empty;


                RecuperarFoco();

                return;
            }


            try
            {
                _procesando =
                    true;


                txtCodigoMarcaje.IsEnabled =
                    false;


                btnRegistrarCodigo.IsEnabled =
                    false;


                MostrarEstadoProcesando();


                var dto =
                    new RegistrarMarcacionAutomaticaDto
                    {
                        TipoCredencial =
                            TipoCredencialMarcaje,

                        Codigo =
                            codigo,

                        Origen =
                            "FERRECONTROL_CARNET_CODIGO_BARRAS"
                    };


                var resultado =
                    await _marcacionApi
                        .RegistrarAutomaticoAsync(
                            dto);


                // Solo protegemos contra doble lectura
                // si la API realmente guardó la marcación.

                _ultimasLecturasExitosas[codigo] =
                    DateTime.Now;


                MostrarResultado(
                    resultado);


                MostrarEstadoExito(
                    resultado);
            }
            catch (Exception ex)
            {
                MostrarEstadoError(
                    LimpiarMensajeError(
                        ex.Message));
            }
            finally
            {
                _procesando =
                    false;


                txtCodigoMarcaje.Text =
                    string.Empty;


                txtCodigoMarcaje.IsEnabled =
                    true;


                btnRegistrarCodigo.IsEnabled =
                    true;


                LimpiarLecturasAntiguas();


                RecuperarFoco();
            }
        }


        // =====================================================
        // PROTECCIÓN DOBLE ESCANEO
        // =====================================================

        private bool EsLecturaDuplicada(
            string codigo)
        {
            if (!_ultimasLecturasExitosas
                .TryGetValue(
                    codigo,
                    out var ultimaLectura))
            {
                return false;
            }


            return DateTime.Now
                -
                ultimaLectura
                <
                TiempoProteccionDobleLectura;
        }


        private void LimpiarLecturasAntiguas()
        {
            var limite =
                DateTime.Now
                    .Subtract(
                        TimeSpan.FromMinutes(5));


            var antiguas =
                _ultimasLecturasExitosas
                    .Where(x =>
                        x.Value <
                        limite)
                    .Select(x =>
                        x.Key)
                    .ToList();


            foreach (var codigo
                in antiguas)
            {
                _ultimasLecturasExitosas
                    .Remove(
                        codigo);
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


            panelResultadoMarcaje.Visibility =
                Visibility.Visible;


            txtResultadoMarcaje.Text =
                ConstruirMensajeResultado(
                    marcacion);
        }


        private static string ConstruirMensajeResultado(
            MarcacionAsistenciaDto marcacion)
        {
            var tipo =
                FormatearTipo(
                    marcacion.TipoMarcacion);


            return marcacion.TipoMarcacion switch
            {
                "ENTRADA" =>
                    $"✓ Bienvenido. {tipo} registrada correctamente.",

                "INICIO_DESCANSO" =>
                    "✓ Salida a descanso registrada correctamente.",

                "FIN_DESCANSO" =>
                    "✓ Regreso de descanso registrado correctamente.",

                "SALIDA" =>
                    "✓ Salida final registrada correctamente.",

                _ =>
                    $"✓ {tipo} registrada correctamente."
            };
        }


        // =====================================================
        // ESTADOS VISUALES
        // =====================================================

        private void MostrarEstadoEsperando()
        {
            ellEstadoMarcaje.Fill =
                ObtenerBrush(
                    "AccentLightBrush");


            txtEstadoMarcaje.Foreground =
                ObtenerBrush(
                    "TextSecondaryBrush");


            txtEstadoMarcaje.Text =
                "Esperando lectura del carné...";
        }


        private void MostrarEstadoProcesando()
        {
            ellEstadoMarcaje.Fill =
                ObtenerBrush(
                    "WarningBrush");


            txtEstadoMarcaje.Foreground =
                ObtenerBrush(
                    "TextSecondaryBrush");


            txtEstadoMarcaje.Text =
                "Validando credencial y registrando marcación...";
        }


        private void MostrarEstadoExito(
            MarcacionAsistenciaDto resultado)
        {
            ellEstadoMarcaje.Fill =
                ObtenerBrush(
                    "SuccessBrush");


            txtEstadoMarcaje.Foreground =
                ObtenerBrush(
                    "SuccessBrush");


            txtEstadoMarcaje.Text =
                $"{FormatearTipo(resultado.TipoMarcacion)} " +
                $"registrada para {resultado.NombreEmpleado}.";
        }


        private void MostrarEstadoAdvertencia(
            string mensaje)
        {
            ellEstadoMarcaje.Fill =
                ObtenerBrush(
                    "WarningBrush");


            txtEstadoMarcaje.Foreground =
                ObtenerBrush(
                    "WarningBrush");


            txtEstadoMarcaje.Text =
                mensaje;
        }


        private void MostrarEstadoError(
            string mensaje)
        {
            ellEstadoMarcaje.Fill =
                ObtenerBrush(
                    "DangerBrush");


            txtEstadoMarcaje.Foreground =
                ObtenerBrush(
                    "DangerBrush");


            txtEstadoMarcaje.Text =
                mensaje;


            panelResultadoMarcaje.Visibility =
                Visibility.Collapsed;
        }


        private Brush ObtenerBrush(
            string recurso)
        {
            return FindResource(
                recurso)
                as Brush
                ??
                Brushes.White;
        }


        // =====================================================
        // FOCO DEL LECTOR
        // =====================================================

        private void RecuperarFoco()
        {
            Dispatcher.BeginInvoke(
                () =>
                {
                    txtCodigoMarcaje.Focus();

                    Keyboard.Focus(
                        txtCodigoMarcaje);


                    txtCodigoMarcaje.CaretIndex =
                        txtCodigoMarcaje.Text.Length;
                },
                DispatcherPriority.Input);
        }


        // =====================================================
        // MENSAJES DE ERROR
        // =====================================================

        private static string LimpiarMensajeError(
            string mensaje)
        {
            if (string.IsNullOrWhiteSpace(
                mensaje))
            {
                return
                    "No se pudo registrar la marcación.";
            }


            var limpio =
                mensaje
                    .Trim()
                    .Trim('"');


            /*
             * Mensajes conocidos del backend.
             */

            if (limpio.Contains(
                "Credencial no encontrada o inactiva",
                StringComparison.OrdinalIgnoreCase))
            {
                return
                    "Credencial inválida o desactivada. " +
                    "Solicite un carné vigente al administrador.";
            }


            if (limpio.Contains(
                "empleado se encuentra inactivo",
                StringComparison.OrdinalIgnoreCase))
            {
                return
                    "El empleado asociado a este carné se encuentra inactivo.";
            }


            if (limpio.Contains(
                "jornada del empleado ya fue finalizada",
                StringComparison.OrdinalIgnoreCase))
            {
                return
                    "La jornada de este empleado ya fue finalizada por hoy.";
            }


            if (limpio.Contains(
                "marcaciones inconsistentes",
                StringComparison.OrdinalIgnoreCase))
            {
                return
                    "La asistencia del empleado necesita revisión administrativa.";
            }


            return limpio;
        }


        // =====================================================
        // INICIALES
        // =====================================================

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
                    StringSplitOptions
                        .RemoveEmptyEntries);


            if (partes.Length ==
                1)
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


        // =====================================================
        // FORMATO TIPO MARCACIÓN
        // =====================================================

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


        // =====================================================
        // FORMATO MÉTODO
        // =====================================================

        private static string FormatearMetodo(
            string metodo)
        {
            return metodo switch
            {
                "CODIGO_BARRAS" =>
                    "Carné · Código de barras",

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