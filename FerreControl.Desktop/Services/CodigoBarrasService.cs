using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace FerreControl.Desktop.Services
{
    public class CodigoBarrasService
    {
        public BitmapSource GenerarCode128(
            string codigo,
            int ancho = 700,
            int alto = 170)
        {
            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                throw new ArgumentException(
                    "El código no puede estar vacío.",
                    nameof(codigo));
            }


            var writer =
                new BarcodeWriterPixelData
                {
                    Format =
                        BarcodeFormat.CODE_128,

                    Options =
                        new EncodingOptions
                        {
                            Width =
                                ancho,

                            Height =
                                alto,

                            Margin =
                                8,

                            PureBarcode =
                                true
                        }
                };


            var pixelData =
                writer.Write(
                    codigo);


            var bitmap =
                BitmapSource.Create(
                    pixelData.Width,
                    pixelData.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    pixelData.Pixels,
                    pixelData.Width * 4);


            bitmap.Freeze();


            return bitmap;
        }
    }
}