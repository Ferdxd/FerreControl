using FerreControl.Application.DTOs.EmpleadoCredencial;

namespace FerreControl.Desktop.Models
{
    public class CarnetEmpleadoFila
    {
        public int EmpleadoID { get; set; }

        public int NegocioID { get; set; }

        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string NombreCompleto { get; set; }
            = string.Empty;

        public string? Puesto { get; set; }

        public int? CredencialID { get; set; }

        public string? CodigoCredencial { get; set; }

        public string? TipoCredencial { get; set; }

        public DateTime? FechaCredencial { get; set; }


        public bool TieneCodigoBarras =>
            CredencialID.HasValue
            &&
            !string.IsNullOrWhiteSpace(
                CodigoCredencial);


        public string EstadoCredencial =>
            TieneCodigoBarras
                ?
                "ACTIVO"
                :
                "SIN CREDENCIAL";


        public string TipoCredencialTexto =>
            TipoCredencial switch
            {
                "CODIGO_BARRAS" =>
                    "Código de barras",

                "QR" =>
                    "QR",

                "RFID" =>
                    "RFID",

                "HUELLA" =>
                    "Huella",

                _ =>
                    "--"
            };


        public static CarnetEmpleadoFila
            DesdeDto(
                CarnetEmpleadoDto dto)
        {
            return new CarnetEmpleadoFila
            {
                EmpleadoID =
                    dto.EmpleadoID,

                NegocioID =
                    dto.NegocioID,

                CodigoEmpleado =
                    dto.CodigoEmpleado,

                NombreCompleto =
                    dto.NombreCompleto,

                Puesto =
                    dto.Puesto,

                CredencialID =
                    dto.CredencialID,

                CodigoCredencial =
                    dto.CodigoCredencial,

                TipoCredencial =
                    dto.TipoCredencial,

                FechaCredencial =
                    dto.FechaCredencial
            };
        }
    }
}