namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class RegistrarAsistenciaManualDto
    {
        public int EmpleadoID { get; set; }

        public DateTime Fecha { get; set; }


        public TimeOnly? HoraEntrada { get; set; }

        public TimeOnly? HoraInicioDescanso { get; set; }

        public TimeOnly? HoraFinDescanso { get; set; }

        public TimeOnly? HoraSalida { get; set; }


        public string? Observacion { get; set; }
    }
}