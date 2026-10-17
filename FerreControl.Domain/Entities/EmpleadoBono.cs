using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class EmpleadoBono
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public int BonoID { get; set; }

        public DateTime FechaInicioPeriodo { get; set; }

        public DateTime FechaFinPeriodo { get; set; }

        public long MontoCentavos { get; set; }

        public bool Cumplio { get; set; }

        public string? Observacion { get; set; }

        public DateTime FechaCalculo { get; set; } = DateTime.Now;

        public Empleado Empleado { get; set; } = null!;

        public Bono Bono { get; set; } = null!;
    }
}
