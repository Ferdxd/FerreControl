using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class RegistrarMarcacionDto
    {
        public string TipoCredencial { get; set; }
            = string.Empty;

        public string Codigo { get; set; }
            = string.Empty;

        public string TipoMarcacion { get; set; }
            = string.Empty;

        public string? Origen { get; set; }
    }
}
