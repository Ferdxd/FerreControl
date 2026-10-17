using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class MarcacionAsistenciaDto
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public int? EmpleadoHorarioID { get; set; }

        public DateTime FechaHora { get; set; }

        public string TipoMarcacion { get; set; }
            = string.Empty;

        public string MetodoMarcacion { get; set; }
            = string.Empty;

        public string? Origen { get; set; }
    }
}
