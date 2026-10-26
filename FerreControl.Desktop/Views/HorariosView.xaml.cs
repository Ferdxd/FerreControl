using FerreControl.Application.DTOs.EmpleadoHorario;
using FerreControl.Application.DTOs.Empleados;
using FerreControl.Application.DTOs.HorarioDetalle;
using FerreControl.Application.DTOs.Horarios;
using FerreControl.Desktop.Models;
using FerreControl.Desktop.Services;
using FerreControl.Desktop.Views.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop.Views
{
    public partial class HorariosView: UserControl
    {
        private readonly HorarioApiService _horarioApi;
        private readonly DetalleHorarioApiService _detalleHorarioApi;
        private readonly EmpleadoApiService _empleadoApi;
        private readonly EmpleadoHorarioApiService _empleadoHorarioApi;

        private List<HorarioDto>  _horarios;
        private List<HorarioDetalleDto>  _detalleHorarios;
        private List<EmpleadoDto> _empleados;
        private List<HorarioEmpleadoFila> _asignacionesActuales;
        private const int NegocioID = 1;


        public HorariosView()
        {
            InitializeComponent();

            _horarioApi =new HorarioApiService();
            _detalleHorarioApi =new DetalleHorarioApiService();
            _empleadoApi = new EmpleadoApiService();
            _empleadoHorarioApi =new EmpleadoHorarioApiService();

            _horarios = new List<HorarioDto>();
            _detalleHorarios = new List<HorarioDetalleDto>();
            _empleados =new List<EmpleadoDto>();
            _asignacionesActuales = new List<HorarioEmpleadoFila>();


            Loaded += UserControl_Loaded;
        }


        // =====================================================
        // CARGA INICIAL
        // =====================================================

        private async void UserControl_Loaded(object sender,RoutedEventArgs e)
        {
            await CargarDatosAsync();
        }


        private async Task CargarDatosAsync(int? seleccionarHorarioId = null)
        {
            try
            {
                // =================================================
                // EMPLEADOS
                // =================================================

                _empleados =(await _empleadoApi.GetObtenerTodos())
                    .Where(x =>
                        x.NegocioID ==
                        NegocioID)
                    .ToList();
                // =================================================
                // HORARIOS
                // =================================================
                _horarios =
                    await _horarioApi
                        .ObtenerPorNegocioAsync(
                            NegocioID);
                // =================================================
                // ASIGNACIONES ACTUALES
                // =================================================

                await CargarAsignacionesActualesAsync();


                ActualizarIndicadores();


                AplicarFiltroHorarios();


                if (_horarios.Count == 0)
                {
                    LimpiarDetalle();

                    return;
                }


                HorarioDto? seleccionar =
                    null;


                if (seleccionarHorarioId.HasValue)
                {
                    seleccionar =
                        _horarios
                            .FirstOrDefault(x =>
                                x.ID ==
                                seleccionarHorarioId.Value);
                }


                seleccionar ??=_horarios.FirstOrDefault();


                if (seleccionar != null)
                {
                    lstHorarios.SelectedItem =
                        seleccionar;
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


        // =====================================================
        // ASIGNACIONES ACTUALES
        // =====================================================

        private async Task CargarAsignacionesActualesAsync()
        {
            _asignacionesActuales =
                new List<HorarioEmpleadoFila>();


            var empleadosActivos =
                _empleados
                    .Where(x =>
                        x.Estado)
                    .ToList();
            /*
             * Actualmente la API consulta el horario por empleado.
             *
             * Para el tamaño de FerreControl esto funciona bien.
             * Más adelante podemos crear un endpoint masivo para
             * evitar una petición por cada empleado.
             */

            foreach (var empleado
                in empleadosActivos)
            {
                var asignacion =
                    await _empleadoHorarioApi
                        .ObtenerActualAsync(
                            empleado.ID);
                if (asignacion == null)
                    continue;


                _asignacionesActuales.Add(
                    new HorarioEmpleadoFila
                    {
                        EmpleadoHorarioID =
                            asignacion.ID,

                        EmpleadoID =
                            empleado.ID,

                        HorarioID =
                            asignacion.HorarioID,

                        Codigo =
                            empleado.Codigo,

                        Empleado =
                            $"{empleado.Nombres} {empleado.Apellidos}"
                                .Trim(),

                        Puesto =
                            empleado.Puesto
                            ??
                            string.Empty,

                        Horario =
                            asignacion.HorarioNombre,

                        FechaInicio =
                            asignacion.FechaInicio
                    });
            }
        }


        // =====================================================
        // INDICADORES
        // =====================================================

        private void ActualizarIndicadores()
        {
            var empleadosActivos =
                _empleados
                    .Where(x =>
                        x.Estado)
                    .ToList();


            int empleadosConHorario =
                _asignacionesActuales
                    .Select(x =>
                        x.EmpleadoID)
                    .Distinct()
                    .Count();


            int empleadosSinHorario =
                Math.Max(
                    0,
                    empleadosActivos.Count
                    -
                    empleadosConHorario);


            txtHorariosActivos.Text =
                _horarios
                    .Count(x =>
                        x.Estado)
                    .ToString();


            txtEmpleadosAsignados.Text =
                empleadosConHorario
                    .ToString();


            txtEmpleadosSinHorario.Text =
                empleadosSinHorario
                    .ToString();


            var horariosActivos =
                _horarios
                    .Where(x =>
                        x.Estado)
                    .ToList();


            double promedio =
                horariosActivos.Count == 0
                    ?
                    0
                    :
                    horariosActivos
                        .Average(x =>
                            x.ToleranciaMinutos);


            txtToleranciaPromedio.Text =
                promedio.ToString(
                    "0.#");
        }


        // =====================================================
        // FILTRO HORARIOS
        // =====================================================

        private void AplicarFiltroHorarios()
        {
            var texto =
                txtBuscarHorario.Text
                    .Trim();


            IEnumerable<HorarioDto>
                resultado =
                    _horarios;


            if (!string.IsNullOrWhiteSpace(
                texto))
            {
                resultado =
                    resultado.Where(x =>
                        x.Nombre.Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase));
            }


            lstHorarios.ItemsSource =
                resultado.ToList();
        }


        private void TxtBuscarHorario_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            AplicarFiltroHorarios();
        }


        // =====================================================
        // SELECCIÓN HORARIO
        // =====================================================

        private async void LstHorarios_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (lstHorarios.SelectedItem
                is not HorarioDto horario)
            {
                LimpiarDetalle();

                return;
            }


            btnEditarHorario.IsEnabled =
                true;


            btnAsignarEmpleado.IsEnabled =
                horario.Estado;


            MostrarHorario(
                horario);


            MostrarEmpleadosHorario(
                horario.ID);


            await CargarDetalleHorarioAsync(
                horario.ID);
        }


        // =====================================================
        // MOSTRAR HORARIO
        // =====================================================

        private void MostrarHorario(
            HorarioDto horario)
        {
            txtNombreHorarioSeleccionado.Text =
                horario.Nombre;


            txtNombreHorario.Text =
                horario.Nombre;


            txtToleranciaHorario.Text =
                $"{horario.ToleranciaMinutos} minutos";


            txtEstadoHorario.Text =
                horario.Estado
                    ?
                    "Activo"
                    :
                    "Inactivo";


            txtEstadoHorario.Foreground =
                horario.Estado
                    ?
                    FindResource(
                        "SuccessBrush")
                        as System.Windows.Media.Brush
                    :
                    FindResource(
                        "DangerBrush")
                        as System.Windows.Media.Brush;
        }


        // =====================================================
        // DETALLE SEMANAL
        // =====================================================

        private async Task CargarDetalleHorarioAsync(
            int horarioId)
        {
            try
            {
                _detalleHorarios =
                    await _detalleHorarioApi
                        .ObtenerPorHorarioAsync(
                            horarioId);


                dgHorarioDetalle.ItemsSource =
                    _detalleHorarios
                        .OrderBy(x =>
                            x.DiaSemana)
                        .Select(x =>
                            new
                            {
                                x.ID,

                                Dia =
                                    ObtenerNombreDia(
                                        x.DiaSemana),

                                x.EsLaboral,

                                Entrada =
                                    x.HoraEntrada?
                                        .ToString(
                                            "HH:mm")
                                    ??
                                    "--",

                                Salida =
                                    x.HoraSalida?
                                        .ToString(
                                            "HH:mm")
                                    ??
                                    "--",

                                Descanso =
                                    x.EsLaboral
                                        ?
                                        $"{x.MinutosDescanso} min"
                                        :
                                        "--",

                                x.MinutosDescanso
                            })
                        .ToList();
            }
            catch (Exception ex)
            {
                dgHorarioDetalle.ItemsSource =
                    null;


                MessageBox.Show(
                    $"No se pudo cargar el detalle del horario.\n\n{ex.Message}",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // EMPLEADOS DEL HORARIO
        // =====================================================

        private void MostrarEmpleadosHorario(
            int horarioId)
        {
            var empleados =
                _asignacionesActuales
                    .Where(x =>
                        x.HorarioID ==
                        horarioId)
                    .OrderBy(x =>
                        x.Empleado)
                    .ToList();


            dgEmpleadosHorario.ItemsSource =
                empleados;


            txtEmpleadosHorario.Text =
                empleados.Count
                    .ToString();


            txtSinEmpleadosHorario.Text =
                empleados.Count == 0
                    ?
                    "Sin empleados asignados"
                    :
                    string.Empty;
        }


        // =====================================================
        // NUEVO HORARIO
        // =====================================================

        private async void BtnNuevoHorario_Click(
            object sender,
            RoutedEventArgs e)
        {
            var ventana =
                new HorarioFormWindow(
                    _horarioApi,
                    _detalleHorarioApi,
                    NegocioID)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                await CargarDatosAsync();
            }
        }


        // =====================================================
        // EDITAR HORARIO
        // =====================================================

        private async void BtnEditarHorario_Click(
            object sender,
            RoutedEventArgs e)
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
                    horario)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true)
            {
                await CargarDatosAsync(
                    horario.ID);
            }
        }


        // =====================================================
        // ASIGNAR EMPLEADO
        // =====================================================

        private async void BtnAsignarEmpleado_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstHorarios.SelectedItem
                is not HorarioDto horario)
            {
                MessageBox.Show(
                    "Seleccione primero un horario.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            if (!horario.Estado)
            {
                MessageBox.Show(
                    "No se pueden asignar empleados a un horario inactivo.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var empleadosActivos =
                _empleados
                    .Where(x =>
                        x.Estado
                        &&
                        x.NegocioID ==
                        NegocioID)
                    .ToList();


            if (empleadosActivos.Count == 0)
            {
                MessageBox.Show(
                    "No existen empleados activos para asignar.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            var ventana =
                new AsignarHorarioEmpleadoWindow(
                    horario,
                    empleadosActivos)
                {
                    Owner =
                        Window.GetWindow(this)
                };


            if (ventana.ShowDialog() == true
                &&
                ventana.HorarioAsignado)
            {
                await CargarDatosAsync(
                    horario.ID);
            }
        }


        // =====================================================
        // LIMPIAR
        // =====================================================

        private void LimpiarDetalle()
        {
            btnEditarHorario.IsEnabled =
                false;


            btnAsignarEmpleado.IsEnabled =
                false;


            txtNombreHorarioSeleccionado.Text =
                "Sin horario seleccionado";


            txtNombreHorario.Text =
                "--";


            txtToleranciaHorario.Text =
                "--";


            txtEmpleadosHorario.Text =
                "--";


            txtEstadoHorario.Text =
                "--";


            txtSinEmpleadosHorario.Text =
                string.Empty;


            dgHorarioDetalle.ItemsSource =
                null;


            dgEmpleadosHorario.ItemsSource =
                null;
        }


        // =====================================================
        // NOMBRE DÍA
        // =====================================================

        private static string ObtenerNombreDia(
            int dia)
        {
            return dia switch
            {
                1 =>
                    "Lunes",

                2 =>
                    "Martes",

                3 =>
                    "Miércoles",

                4 =>
                    "Jueves",

                5 =>
                    "Viernes",

                6 =>
                    "Sábado",

                7 =>
                    "Domingo",

                _ =>
                    "Desconocido"
            };
        }
    }
}