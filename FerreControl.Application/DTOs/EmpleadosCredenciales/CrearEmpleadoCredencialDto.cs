using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.EmpleadoCredencial
{
    public class CrearEmpleadoCredencialDto
    {
        public string TipoCredencial { get; set; } = string.Empty;

        public string Codigo { get; set; }= string.Empty;
    }
}