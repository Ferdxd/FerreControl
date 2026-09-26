using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.Salarios
{
    public class SalarioEmpleadoDto
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public decimal Monto { get; set; }

        public string Periodicidad { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Estado { get; set; }
    }
}
