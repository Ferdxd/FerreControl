using FerreControl.Desktop.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace FerreControl.Desktop.Services
{
    public class CarnetPrintService
    {
        private const double Mm =
            96.0 / 25.4;


        private const double AnchoCarnet =
            85.6 * Mm;

        private const double AltoCarnet =
            54.0 * Mm;


        private readonly CodigoBarrasService
            _codigoBarrasService;


        public CarnetPrintService(
            CodigoBarrasService codigoBarrasService)
        {
            _codigoBarrasService =
                codigoBarrasService;
        }


        public bool Imprimir(
            IEnumerable<CarnetEmpleadoFila> carnets)
        {
            var lista =
                carnets
                    .Where(x =>
                        x.TieneCodigoBarras)
                    .ToList();


            if (lista.Count == 0)
            {
                MessageBox.Show(
                    "No existen carnés con código de barras para imprimir.",
                    "FerreControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return false;
            }


            var dialogo =
                new PrintDialog();


            if (dialogo.ShowDialog() !=
                true)
            {
                return false;
            }


            double anchoPagina =
                dialogo.PrintableAreaWidth;

            double altoPagina =
                dialogo.PrintableAreaHeight;


            if (anchoPagina <= 0)
            {
                anchoPagina =
                    8.5 * 96;
            }


            if (altoPagina <= 0)
            {
                altoPagina =
                    11 * 96;
            }


            var documento =
                ConstruirDocumento(
                    lista,
                    anchoPagina,
                    altoPagina);


            dialogo.PrintDocument(
                documento.DocumentPaginator,
                "Carnés FerreControl");


            return true;
        }


        // =====================================================
        // DOCUMENTO
        // =====================================================

        private FixedDocument
            ConstruirDocumento(
                List<CarnetEmpleadoFila> carnets,
                double anchoPagina,
                double altoPagina)
        {
            var documento =
                new FixedDocument();


            documento.DocumentPaginator.PageSize =
                new Size(
                    anchoPagina,
                    altoPagina);


            const double margen =
                24;

            const double separacion =
                14;


            int columnas =
                Math.Max(
                    1,
                    (int)(
                        (
                            anchoPagina
                            -
                            margen * 2
                            +
                            separacion
                        )
                        /
                        (
                            AnchoCarnet
                            +
                            separacion
                        )));


            columnas =
                Math.Min(
                    columnas,
                    2);


            int filas =
                Math.Max(
                    1,
                    (int)(
                        (
                            altoPagina
                            -
                            margen * 2
                            +
                            separacion
                        )
                        /
                        (
                            AltoCarnet
                            +
                            separacion
                        )));


            int porPagina =
                columnas * filas;


            for (int inicio = 0;
                inicio < carnets.Count;
                inicio += porPagina)
            {
                var pagina =
                    new FixedPage
                    {
                        Width =
                            anchoPagina,

                        Height =
                            altoPagina,

                        Background =
                            Brushes.White
                    };


                var grupo =
                    carnets
                        .Skip(
                            inicio)
                        .Take(
                            porPagina)
                        .ToList();


                for (int i = 0;
                    i < grupo.Count;
                    i++)
                {
                    int fila =
                        i / columnas;

                    int columna =
                        i % columnas;


                    double izquierda =
                        margen
                        +
                        columna
                        *
                        (
                            AnchoCarnet
                            +
                            separacion
                        );


                    double arriba =
                        margen
                        +
                        fila
                        *
                        (
                            AltoCarnet
                            +
                            separacion
                        );


                    var tarjeta =
                        CrearTarjeta(
                            grupo[i]);


                    FixedPage.SetLeft(
                        tarjeta,
                        izquierda);


                    FixedPage.SetTop(
                        tarjeta,
                        arriba);


                    pagina.Children.Add(
                        tarjeta);
                }


                var contenido =
                    new PageContent();


                ((IAddChild)contenido)
                    .AddChild(
                        pagina);


                documento.Pages.Add(
                    contenido);
            }


            return documento;
        }


        // =====================================================
        // TARJETA
        // =====================================================

        private Border CrearTarjeta(
            CarnetEmpleadoFila carnet)
        {
            var contenedor =
                new Grid
                {
                    Width =
                        AnchoCarnet,

                    Height =
                        AltoCarnet,

                    Background =
                        Brushes.White
                };


            contenedor.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        new GridLength(34)
                });


            contenedor.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        new GridLength(64)
                });


            contenedor.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        new GridLength(1,
                            GridUnitType.Star)
                });


            // HEADER

            var header =
                new Border
                {
                    Background =
                        new SolidColorBrush(
                            Color.FromRgb(
                                15,
                                23,
                                42))
                };


            var headerGrid =
                new Grid
                {
                    Margin =
                        new Thickness(
                            12,
                            0,
                            12,
                            0)
                };


            headerGrid.ColumnDefinitions.Add(
                new ColumnDefinition());


            headerGrid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width =
                        GridLength.Auto
                });


            var marca =
                new TextBlock
                {
                    Text =
                        "FERRECONTROL",

                    Foreground =
                        Brushes.White,

                    FontWeight =
                        FontWeights.Bold,

                    FontSize =
                        12,

                    VerticalAlignment =
                        VerticalAlignment.Center
                };


            var tipo =
                new TextBlock
                {
                    Text =
                        "EMPLEADO",

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                148,
                                163,
                                184)),

                    FontSize =
                        8,

                    FontWeight =
                        FontWeights.SemiBold,

                    VerticalAlignment =
                        VerticalAlignment.Center
                };


            Grid.SetColumn(
                tipo,
                1);


            headerGrid.Children.Add(
                marca);

            headerGrid.Children.Add(
                tipo);


            header.Child =
                headerGrid;


            Grid.SetRow(
                header,
                0);


            contenedor.Children.Add(
                header);


            // DATOS

            var datos =
                new StackPanel
                {
                    Margin =
                        new Thickness(
                            12,
                            8,
                            12,
                            4)
                };


            datos.Children.Add(
                new TextBlock
                {
                    Text =
                        carnet.NombreCompleto
                            .ToUpperInvariant(),

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                15,
                                23,
                                42)),

                    FontSize =
                        13,

                    FontWeight =
                        FontWeights.Bold,

                    TextTrimming =
                        TextTrimming.CharacterEllipsis
                });


            datos.Children.Add(
                new TextBlock
                {
                    Text =
                        carnet.Puesto
                        ??
                        "Empleado",

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                71,
                                85,
                                105)),

                    FontSize =
                        9,

                    Margin =
                        new Thickness(
                            0,
                            3,
                            0,
                            0)
                });


            datos.Children.Add(
                new TextBlock
                {
                    Text =
                        $"Código: {carnet.CodigoEmpleado}",

                    Foreground =
                        new SolidColorBrush(
                            Color.FromRgb(
                                100,
                                116,
                                139)),

                    FontSize =
                        8,

                    Margin =
                        new Thickness(
                            0,
                            4,
                            0,
                            0)
                });


            Grid.SetRow(
                datos,
                1);


            contenedor.Children.Add(
                datos);


            // BARCODE

            var barcodePanel =
                new StackPanel
                {
                    Margin =
                        new Thickness(
                            12,
                            2,
                            12,
                            8)
                };


            var imagen =
                new Image
                {
                    Source =
                        _codigoBarrasService
                            .GenerarCode128(
                                carnet.CodigoCredencial!,
                                700,
                                150),

                    Height =
                        54,

                    Stretch =
                        Stretch.Fill
                };


            barcodePanel.Children.Add(
                imagen);


            barcodePanel.Children.Add(
                new TextBlock
                {
                    Text =
                        carnet.CodigoCredencial,

                    Foreground =
                        Brushes.Black,

                    FontFamily =
                        new FontFamily(
                            "Consolas"),

                    FontSize =
                        7,

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    Margin =
                        new Thickness(
                            0,
                            3,
                            0,
                            0)
                });


            Grid.SetRow(
                barcodePanel,
                2);


            contenedor.Children.Add(
                barcodePanel);


            return new Border
            {
                Width =
                    AnchoCarnet,

                Height =
                    AltoCarnet,

                Background =
                    Brushes.White,

                BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            148,
                            163,
                            184)),

                BorderThickness =
                    new Thickness(1),

                CornerRadius =
                    new CornerRadius(5),

                ClipToBounds =
                    true,

                Child =
                    contenedor
            };
        }
    }
}