namespace FerreControl.Domain.Entities
{
    public class PagoNomina
    {
        public int ID { get; set; }
        public int NominaDetalleID { get; set; }
        public long MontoCentavos { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        // EFECTIVO, TRANSFERENCIA, CHEQUE, etc.
        public string MetodoPago { get; set; }= string.Empty;

        public string? Referencia { get; set; }

        public string? Observacion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; }= DateTime.Now;

        public NominaDetalle NominaDetalle { get; set; }
            = null!;
    }
}