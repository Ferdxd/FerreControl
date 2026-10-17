using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class Horario
    {
        public int ID { get; set; }
        public int NegocioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int ToleranciaMinutos { get; set; } = 0;
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Negocio Negocio { get; set; } = null!;

        public ICollection<HorarioDetalle> Detalles { get; set; } = new List<HorarioDetalle>();
        public ICollection<EmpleadoHorario> EmpleadosHorarios { get; set; } = new List<EmpleadoHorario>();
    }
}
