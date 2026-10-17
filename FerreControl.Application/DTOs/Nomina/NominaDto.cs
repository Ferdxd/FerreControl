namespace FerreControl.Application.DTOs.Nomina
{
    public class NominaDto
    {
        public int ID { get; set; }

        public int NegocioID { get; set; }

        public DateTime FechaInicioPeriodo { get; set; }

        public DateTime FechaFinPeriodo { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}