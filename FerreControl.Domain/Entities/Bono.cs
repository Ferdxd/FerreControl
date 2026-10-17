using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class Bono
    {
        public int ID { get; set; }

        public int NegocioID { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string TipoAplicacion { get; set; } = string.Empty;

        public string TipoCalculo { get; set; } = string.Empty;

        public long MontoCentavos { get; set; }

        public string Periodicidad { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Negocio Negocio { get; set; } = null!;
    }
}
