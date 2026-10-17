namespace FerreControl.Application.DTOs.Nomina
{
    public class PagoNominaDto
    {
        public int ID { get; set; }

        public int NominaDetalleID { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public string? Referencia { get; set; }

        public string? Observacion { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}