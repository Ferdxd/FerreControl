using FerreControl.Desktop.Views;
using System.Windows;
using System.Windows.Controls;

namespace FerreControl.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MostrarVista(
                new EmpleadosView(),
                "Empleados");
        }


        private void MostrarVista(
            UserControl vista,
            string titulo)
        {
            MainContent.Content = vista;

            txtTituloPagina.Text = titulo;
        }


        private void BtnMarcaje_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new MarcajeView(),
                "Marcaje");
        }


        private void BtnEmpleados_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new EmpleadosView(),
                "Empleados");
        }


        private void BtnHorarios_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new HorariosView(),
                "Horarios");
        }

        private void BtnSalarios_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new SalariosView(),
                "Salarios");
        }


        private void BtnBonos_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new BonosView(),
                "Bonos");
        }


        private void BtnAsistencia_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarVista(
                new AsistenciaView(),
                "Asistencia");
        }

        private void BtnNomina_Click( object sender, RoutedEventArgs e)
        {
            MostrarVista(new NominaView(), "Nómina");
        }
    }
}