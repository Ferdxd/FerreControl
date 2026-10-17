namespace FerreControl.Desktop.Models
{
    public class SalarioEmpleadoFila
    {
        public int EmpleadoID { get; set; }

        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string NombreEmpleado { get; set; }
            = string.Empty;

        public string? Puesto { get; set; }

        public decimal? Monto { get; set; }

        public string Periodicidad { get; set; }
            = string.Empty;

        public DateTime? FechaInicio { get; set; }

        public bool TieneSalario { get; set; }

        public bool EmpleadoActivo { get; set; }
    }
}