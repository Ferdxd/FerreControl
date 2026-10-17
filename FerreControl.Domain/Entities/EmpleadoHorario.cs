using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class EmpleadoHorario
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public int HorarioID { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Estado { get; set; } = true;


        public Empleado Empleado { get; set; } = null!;

        public Horario Horario { get; set; } = null!;

        public ICollection<MarcacionAsistencia> Marcaciones { get; set; }
            = new List<MarcacionAsistencia>();
    }
}
