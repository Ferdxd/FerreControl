using FerreControl.Application.DTOs.HorarioDetalle;
using FerreControl.Application.DTOs.Horarios;
using FerreControl.Desktop.Services;
using System.Windows;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class HorarioFormWindow : Window
    {
        private readonly HorarioApiService _horarioApi;
        private readonly DetalleHorarioApiService _detalleApi;

        private readonly HorarioDto? _horario;

        private readonly int _negocioId;

        private List<DiaHorarioForm> _dias;


        // =========================================================
        // NUEVO HORARIO
        // =========================================================

        public HorarioFormWindow(
            HorarioApiService horarioApi,
            DetalleHorarioApiService detalleApi,
            int negocioId)
        {
            InitializeComponent();

            _horarioApi = horarioApi;
            _detalleApi = detalleApi;
            _negocioId = negocioId;

            _dias = CrearSemanaPorDefecto();

            dgDias.ItemsSource = _dias;
        }


        // =========================================================
        // EDITAR HORARIO
        // =========================================================

        public HorarioFormWindow(
            HorarioApiService horarioApi,
            DetalleHorarioApiService detalleApi,
            HorarioDto horario)
        {
            InitializeComponent();

            _horarioApi = horarioApi;
            _detalleApi = detalleApi;

            _horario = horario;
            _negocioId = horario.NegocioID;

            txtTitulo.Text = "Editar horario";

            txtSubtitulo.Text =
                "Modifica la jornada, tolerancia y días laborales.";

            txtNombre.Text = horario.Nombre;

            txtTolerancia.Text =
                horario.ToleranciaMinutos.ToString();

            chkEstado.IsChecked =
                horario.Estado;

            panelEstado.Visibility =
                Visibility.Visible;

            _dias = CrearSemanaVacia();

            dgDias.ItemsSource = _dias;

            Loaded += HorarioFormWindow_Loaded;
        }


        // =========================================================
        // CARGAR DETALLE AL EDITAR
        // =========================================================

        private async void HorarioFormWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (_horario == null)
                return;

            try
            {
                var detalles =
                    await _detalleApi
                        .ObtenerPorHorarioAsync(
                            _horario.ID);


                foreach (var dia in _dias)
                {
                    var detalle =
                        detalles.FirstOrDefault(
                            x => x.DiaSemana == dia.DiaSemana);

                    if (detalle == null)
                        continue;


                    dia.DetalleID =
                        detalle.ID;

                    dia.EsLaboral =
                        detalle.EsLaboral;

                    dia.HoraEntrada =
                        detalle.HoraEntrada?
                            .ToString("HH:mm")
                        ?? "";

                    dia.HoraSalida =
                        detalle.HoraSalida?
                            .ToString("HH:mm")
                        ?? "";

                    dia.MinutosDescanso =
                        detalle.MinutosDescanso;
                }


                dgDias.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cargar la configuración semanal.\n\n{ex.Message}",
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
                if (string.IsNullOrWhiteSpace(
                    txtNombre.Text))
                {
                    MostrarAdvertencia(
                        "Debe ingresar el nombre del horario.");

                    return;
                }


                if (!int.TryParse(
                    txtTolerancia.Text,
                    out int tolerancia)
                    || tolerancia < 0)
                {
                    MostrarAdvertencia(
                        "La tolerancia debe ser un número válido.");

                    return;
                }


                if (!ValidarDias())
                    return;


                btnGuardar.IsEnabled = false;


                int horarioId;


                // =================================================
                // CREAR
                // =================================================

                if (_horario == null)
                {
                    var dto =
                        new CrearHorarioDto
                        {
                            NegocioID =
                                _negocioId,

                            Nombre =
                                txtNombre.Text.Trim(),

                            ToleranciaMinutos =
                                tolerancia
                        };


                    var horarioCreado =
                        await _horarioApi
                            .CrearAsync(dto);


                    if (horarioCreado == null)
                    {
                        throw new InvalidOperationException(
                            "No se pudo crear el horario.");
                    }


                    horarioId =
                        horarioCreado.ID;
                }

                // =================================================
                // ACTUALIZAR
                // =================================================

                else
                {
                    var dto =
                        new ActualizarHorarioDto
                        {
                            Nombre =
                                txtNombre.Text.Trim(),

                            ToleranciaMinutos =
                                tolerancia,

                            Estado =
                                chkEstado.IsChecked
                                ?? false
                        };


                    await _horarioApi
                        .ActualizarAsync(
                            _horario.ID,
                            dto);


                    horarioId =
                        _horario.ID;
                }


                // =================================================
                // GUARDAR LOS 7 DÍAS
                // =================================================

                foreach (var dia in _dias)
                {
                    TimeOnly? entrada = null;
                    TimeOnly? salida = null;


                    if (dia.EsLaboral)
                    {
                        entrada =
                            TimeOnly.Parse(
                                dia.HoraEntrada);

                        salida =
                            TimeOnly.Parse(
                                dia.HoraSalida);
                    }


                    // Día nuevo
                    if (dia.DetalleID == null)
                    {
                        var dto =
                            new CrearHorarioDetalleDto
                            {
                                DiaSemana =
                                    dia.DiaSemana,

                                EsLaboral =
                                    dia.EsLaboral,

                                HoraEntrada =
                                    entrada,

                                HoraSalida =
                                    salida,

                                MinutosDescanso =
                                    dia.EsLaboral
                                    ? dia.MinutosDescanso
                                    : 0
                            };


                        await _detalleApi
                            .CrearAsync(
                                horarioId,
                                dto);
                    }

                    // Día existente
                    else
                    {
                        var dto =
                            new ActualizarHorarioDetalleDto
                            {
                                DiaSemana =
                                    dia.DiaSemana,

                                EsLaboral =
                                    dia.EsLaboral,

                                HoraEntrada =
                                    entrada,

                                HoraSalida =
                                    salida,

                                MinutosDescanso =
                                    dia.EsLaboral
                                    ? dia.MinutosDescanso
                                    : 0
                            };


                        await _detalleApi
                            .ActualizarAsync(
                                dia.DetalleID.Value,
                                dto);
                    }
                }


                MessageBox.Show(
                    _horario == null
                        ? "Horario creado correctamente."
                        : "Horario actualizado correctamente.",
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
                btnGuardar.IsEnabled = true;
            }
        }


        // =========================================================
        // VALIDACIÓN
        // =========================================================

        private bool ValidarDias()
        {
            foreach (var dia in _dias)
            {
                if (!dia.EsLaboral)
                    continue;


                if (!TimeOnly.TryParse(
                    dia.HoraEntrada,
                    out TimeOnly entrada))
                {
                    MostrarAdvertencia(
                        $"La hora de entrada de {dia.NombreDia} no es válida.");

                    return false;
                }


                if (!TimeOnly.TryParse(
                    dia.HoraSalida,
                    out TimeOnly salida))
                {
                    MostrarAdvertencia(
                        $"La hora de salida de {dia.NombreDia} no es válida.");

                    return false;
                }


                if (salida <= entrada)
                {
                    MostrarAdvertencia(
                        $"La hora de salida de {dia.NombreDia} debe ser posterior a la entrada.");

                    return false;
                }


                if (dia.MinutosDescanso < 0)
                {
                    MostrarAdvertencia(
                        $"El descanso de {dia.NombreDia} no puede ser negativo.");

                    return false;
                }
            }


            return true;
        }


        private static List<DiaHorarioForm>
            CrearSemanaPorDefecto()
        {
            return new List<DiaHorarioForm>
            {
                new(1, "Lunes", true, "08:00", "17:00", 60),
                new(2, "Martes", true, "08:00", "17:00", 60),
                new(3, "Miércoles", true, "08:00", "17:00", 60),
                new(4, "Jueves", true, "08:00", "17:00", 60),
                new(5, "Viernes", true, "08:00", "17:00", 60),
                new(6, "Sábado", true, "08:00", "12:00", 0),
                new(7, "Domingo", false, "", "", 0)
            };
        }


        private static List<DiaHorarioForm>
            CrearSemanaVacia()
        {
            return new List<DiaHorarioForm>
            {
                new(1, "Lunes"),
                new(2, "Martes"),
                new(3, "Miércoles"),
                new(4, "Jueves"),
                new(5, "Viernes"),
                new(6, "Sábado"),
                new(7, "Domingo")
            };
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


    // =============================================================
    // MODELO SOLO PARA EL FORMULARIO
    // =============================================================

    public class DiaHorarioForm
    {
        public int? DetalleID { get; set; }

        public int DiaSemana { get; set; }

        public string NombreDia { get; set; }

        public bool EsLaboral { get; set; }

        public string HoraEntrada { get; set; }

        public string HoraSalida { get; set; }

        public int MinutosDescanso { get; set; }


        public DiaHorarioForm(
            int diaSemana,
            string nombreDia,
            bool esLaboral = false,
            string horaEntrada = "",
            string horaSalida = "",
            int minutosDescanso = 0)
        {
            DiaSemana = diaSemana;
            NombreDia = nombreDia;
            EsLaboral = esLaboral;
            HoraEntrada = horaEntrada;
            HoraSalida = horaSalida;
            MinutosDescanso = minutosDescanso;
        }
    }
}