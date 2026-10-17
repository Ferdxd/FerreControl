namespace FerreControl.Application.DTOs.Nomina
{
    public class CrearPagoNominaDto
    {
        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public string? Referencia { get; set; }

        public string? Observacion { get; set; }
    }
}