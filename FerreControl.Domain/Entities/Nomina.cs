namespace FerreControl.Domain.Entities
{
    public class Nomina
    {
        public int ID { get; set; }

        public int NegocioID { get; set; }

        public DateTime FechaInicioPeriodo { get; set; }

        public DateTime FechaFinPeriodo { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        // BORRADOR, GENERADA, CERRADA
        public string Estado { get; set; } = "BORRADOR";


        public Negocio Negocio { get; set; } = null!;

        public ICollection<NominaDetalle> Detalles { get; set; }
            = new List<NominaDetalle>();
    }
}