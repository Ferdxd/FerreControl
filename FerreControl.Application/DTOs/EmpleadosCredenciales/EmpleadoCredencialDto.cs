using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FerreControl.Application.DTOs.EmpleadoCredencial
{
    public class EmpleadoCredencialDto
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public string TipoCredencial { get; set; }= string.Empty;

        public string Codigo { get; set; }= string.Empty;

        public bool Estado { get; set; }
    }
}