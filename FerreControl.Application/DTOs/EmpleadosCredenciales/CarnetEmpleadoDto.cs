namespace FerreControl.Application.DTOs.EmpleadoCredencial
{
    public class CarnetEmpleadoDto
    {
        public int EmpleadoID { get; set; }

        public int NegocioID { get; set; }

        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string NombreCompleto { get; set; }
            = string.Empty;

        public string? Puesto { get; set; }

        public bool EmpleadoActivo { get; set; }


        public int? CredencialID { get; set; }

        public string? TipoCredencial { get; set; }

        public string? CodigoCredencial { get; set; }

        public DateTime? FechaCredencial { get; set; }


        public bool TieneCodigoBarras =>
            CredencialID.HasValue
            &&
            !string.IsNullOrWhiteSpace(
                CodigoCredencial);
    }
}