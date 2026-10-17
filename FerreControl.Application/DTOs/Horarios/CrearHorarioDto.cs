using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.Horarios
{
    public class CrearHorarioDto
    {
        public int NegocioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int ToleranciaMinutos { get; set; }
        
    }
}
