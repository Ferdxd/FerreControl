namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class MarcacionAsistenciaDto
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public int? EmpleadoCredencialID { get; set; }

        public int? EmpleadoHorarioID { get; set; }


        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string NombreEmpleado { get; set; }
            = string.Empty;


        public DateTime FechaHora { get; set; }

        public string TipoMarcacion { get; set; }
            = string.Empty;

        public string MetodoMarcacion { get; set; }
            = string.Empty;

        public string? Origen { get; set; }
    }
}