using FerreControl.Application.DTOs.Nomina;
using FerreControl.Desktop.Services;
using System.Windows;

namespace FerreControl.Desktop.Views.Forms
{
    public partial class NominaFormWindow
        : Window
    {
        private readonly NominaApiService
            _nominaApi;

        private readonly int
            _negocioId;


        public int NominaCreadaId { get; private set; }


        public NominaFormWindow(
            int negocioId)
        {
            InitializeComponent();


            _negocioId =
                negocioId;


            _nominaApi =
                new NominaApiService();


            dpFechaInicio.SelectedDate =
                DateTime.Today;


            dpFechaFin.SelectedDate =
                DateTime.Today;
        }


        private async void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!dpFechaInicio.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha de inicio.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!dpFechaFin.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Seleccione la fecha de fin.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            var inicio =
                dpFechaInicio.SelectedDate.Value.Date;

            var fin =
                dpFechaFin.SelectedDate.Value.Date;


            if (inicio > fin)
            {
                MessageBox.Show(
                    "La fecha de inicio no puede ser mayor que la fecha de fin.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                btnGuardar.IsEnabled =
                    false;


                var dto =
                    new CrearNominaDto
                    {
                        NegocioID =
                            _negocioId,

                        FechaInicioPeriodo =
                            inicio,

                        FechaFinPeriodo =
                            fin
                    };


                var resultado =
                    await _nominaApi
                        .CrearAsync(
                            dto);


                NominaCreadaId =
                    resultado.ID;


                MessageBox.Show(
                    "La nómina fue creada correctamente.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo crear la nómina.\n\n{ex.Message}",
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


        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult =
                false;
        }
    }
}