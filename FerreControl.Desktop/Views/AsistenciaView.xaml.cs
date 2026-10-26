using FerreControl.Desktop.Views.Forms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FerreControl.Desktop.Views
{
    public partial class AsistenciaView : UserControl
    {
        public AsistenciaView()
        {
            InitializeComponent();

            dpFechaAsistencia.SelectedDate =
                DateTime.Today;

            cmbEstadoAsistencia.SelectedIndex =
                0;

            /*
             * El botón Exportar todavía no tenía implementación.
             * Temporalmente lo reutilizamos para abrir Horas Extra.
             * Cuando terminemos la exportación, se agrega un botón
             * separado en el XAML.
             */
            btnExportarAsistencia.Content =
                "Horas extra";

            btnExportarAsistencia.IsEnabled =
                true;

            btnExportarAsistencia.Click +=
                BtnHorasExtra_Click;
        }

        private void BtnHorasExtra_Click(
            object sender,
            RoutedEventArgs e)
        {
            var ventana =
                new HorasExtraWindow
                {
                    Owner =
                        Window.GetWindow(this)
                };

            ventana.ShowDialog();
        }

        private void BtnActualizarAsistencia_Click(
            object sender,
            RoutedEventArgs e)
        {
        }

        private void BtnRegistrarManual_Click(
            object sender,
            RoutedEventArgs e)
        {
        }

        private void BtnVerMarcaciones_Click(
            object sender,
            RoutedEventArgs e)
        {
        }

        private void CmbEstadoAsistencia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
        }

        private void DgAsistencia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
        }

        private void TxtBuscarAsistencia_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
        }

        private void DpFechaAsistencia_SelectedDateChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
        }

        private void DgAsistencia_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
        }
    }
}
