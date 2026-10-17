using FerreControl.Application.DTOs.HorarioDetalle;
using FerreControl.Application.DTOs.Horarios;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class HorariosView : UserControl
    {
        private readonly HorarioApiService _horarioApi;
        private readonly DetalleHorarioApiService _detalleHorarioApi;

        private List<HorarioDto> _horarios;
        private List<HorarioDetalleDto> _detalleHorarios;

        private const int NegocioID = 1;


        public HorariosView()
        {
            InitializeComponent();

            _horarioApi =
                new HorarioApiService();

            _detalleHorarioApi =
                new DetalleHorarioApiService();

            _horarios =
                new List<HorarioDto>();

            _detalleHorarios =
                new List<HorarioDetalleDto>();

            Loaded += UserControl_Loaded;
        }


        private async void UserControl_Loaded(object sender,RoutedEventArgs e)
        {
            await CargarHorariosAsync();
        }


        private async Task CargarHorariosAsync()
        {
            try
            {
                _horarios =
                    await _horarioApi
                        .ObtenerPorNegocioAsync(
                            NegocioID);

                lstHorarios.ItemsSource =
                    _horarios;

                txtHorariosActivos.Text =
                    _horarios
                        .Count(x => x.Estado)
                        .ToString();


                if (_horarios.Count > 0)
                {
                    lstHorarios.SelectedIndex = 0;
                }
                else
                {
                    LimpiarDetalle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar los horarios.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private async void LstHorarios_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            if (lstHorarios.SelectedItem is not HorarioDto horario)
            {
                LimpiarDetalle();
                btnEditarHorario.IsEnabled = false;
                return;
            }
            else
            {
                btnEditarHorario.IsEnabled = true;
            }
           


            MostrarHorario(horario);

            await CargarDetalleHorarioAsync(
                horario.ID);
        }


        private void MostrarHorario(HorarioDto horario)
        {
            txtNombreHorarioSeleccionado.Text =
                horario.Nombre;

            txtNombreHorario.Text =
                horario.Nombre;

            txtToleranciaHorario.Text =
                $"{horario.ToleranciaMinutos} minutos";

            txtEstadoHorario.Text =
                horario.Estado
                    ? "Activo"
                    : "Inactivo";
        }


        private async Task CargarDetalleHorarioAsync(int horarioId)
        {
            try
            {
                _detalleHorarios =
                    await _detalleHorarioApi
                        .ObtenerPorHorarioAsync(
                            horarioId);

                dgHorarioDetalle.ItemsSource =
                    _detalleHorarios.Select(x => new
                    {
                        x.ID,

                        Dia = ObtenerNombreDia(
                            x.DiaSemana),

                        x.EsLaboral,

                        Entrada =
                            x.HoraEntrada?
                                .ToString("HH:mm")
                            ?? "--",

                        Salida =
                            x.HoraSalida?
                                .ToString("HH:mm")
                            ?? "--",

                        x.MinutosDescanso
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                dgHorarioDetalle.ItemsSource = null;

                MessageBox.Show(
                    $"No se pudo cargar el detalle.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private static string ObtenerNombreDia(int dia)
        {
            return dia switch
            {
                1 => "Lunes",
                2 => "Martes",
                3 => "Miércoles",
                4 => "Jueves",
                5 => "Viernes",
                6 => "Sábado",
                7 => "Domingo",
                _ => "Desconocido"
            };
        }

        private void LimpiarDetalle()
        {
            txtNombreHorarioSeleccionado.Text =
                "Sin horario seleccionado";

            txtNombreHorario.Text = "--";
            txtToleranciaHorario.Text = "--";
            txtEstadoHorario.Text = "--";

            dgHorarioDetalle.ItemsSource = null;
        }
        private async void BtnNuevoHorario_Click(object sender, RoutedEventArgs e)
        {
            var ventana =
                new HorarioFormWindow(
                    _horarioApi,
                    _detalleHorarioApi,
                    NegocioID);

            ventana.Owner =
                Window.GetWindow(this);


            if (ventana.ShowDialog() == true)
            {
                await CargarHorariosAsync();
            }
        }

        private async void BtnEditarHorario_Click(object sender,RoutedEventArgs e)
        {
            if (lstHorarios.SelectedItem
                is not HorarioDto horario)
            {
                return;
            }


            var ventana =
                new HorarioFormWindow(
                    _horarioApi,
                    _detalleHorarioApi,
                    horario);

            ventana.Owner =
                Window.GetWindow(this);


            if (ventana.ShowDialog() == true)
            {
                await CargarHorariosAsync();
            }
        }

        private void TxtBuscarHorario_TextChanged(object sender,TextChangedEventArgs e)
        {
            var texto =
                txtBuscarHorario.Text
                    .Trim()
                    .ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                lstHorarios.ItemsSource =
                    _horarios;

                return;
            }


            lstHorarios.ItemsSource =
                _horarios
                    .Where(x =>
                        x.Nombre
                            .ToLower()
                            .Contains(texto))
                    .ToList();
        }


    }
}