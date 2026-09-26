using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.Salarios
{
    public class CrearSalarioEmpleadoDto
    {

        public decimal Monto { get; set; }

        public string Periodicidad { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }

    }
}
