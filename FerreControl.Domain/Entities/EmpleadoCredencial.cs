using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class EmpleadoCredencial
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public string TipoCredencial { get; set; }
            = string.Empty;

        public string Codigo { get; set; }
            = string.Empty;

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
            = DateTime.Now;


        public Empleado Empleado { get; set; }
            = null!;

        public ICollection<MarcacionAsistencia>
            Marcaciones
        { get; set; }
            = new List<MarcacionAsistencia>();
    }
}
