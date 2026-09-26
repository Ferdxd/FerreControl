using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.Negocios
{
    public class NegocioDto
    {   
        public int ID {  get; set; }
        public string Nombre { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public bool Estado { get; set; }
    }
}
