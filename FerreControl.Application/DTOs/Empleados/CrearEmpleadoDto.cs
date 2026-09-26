using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.Empleados
{
    public class CrearEmpleadoDto
    {
        public int NegocioID { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Puesto { get; set; }
    }
}
