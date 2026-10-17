using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class HorarioDetalle
    {
        public int ID { get; set; }
        public int HorarioID { get; set; }
        public int DiaSemana { get; set; }
        public bool EsLaboral { get; set; } = true;
        public TimeOnly? HoraEntrada { get; set; }
        public TimeOnly? HoraSalida { get; set; }
        public int MinutosDescanso { get; set; } = 0;

        public Horario Horario { get; set; } = null!;




    }
}
