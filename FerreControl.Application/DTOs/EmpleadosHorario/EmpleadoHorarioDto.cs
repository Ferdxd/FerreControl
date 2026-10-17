using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.EmpleadoHorario
{
    public class EmpleadoHorarioDto
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public int HorarioID { get; set; }

        public string HorarioNombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Estado { get; set; }
    }
}
