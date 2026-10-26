namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class AsistenciaDiariaDto
    {
        public int EmpleadoID { get; set; }

        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string NombreEmpleado { get; set; }
            = string.Empty;


        public string Horario { get; set; }
            = string.Empty;


        public bool EsLaboral { get; set; }

        public TimeOnly? HoraEntradaProgramada { get; set; }

        public TimeOnly? HoraSalidaProgramada { get; set; }


        public DateTime? HoraEntrada { get; set; }

        public DateTime? HoraInicioDescanso { get; set; }

        public DateTime? HoraFinDescanso { get; set; }

        public DateTime? HoraSalida { get; set; }


        public int MinutosProgramados { get; set; }

        public int MinutosTrabajados { get; set; }

        public int MinutosTarde { get; set; }

        public int MinutosFaltantes { get; set; }

        public int MinutosExtraCalculados { get; set; }


        public int ToleranciaMinutos { get; set; }


        public bool EsTarde { get; set; }

        public bool EsPresente { get; set; }

        public bool SinSalida { get; set; }


        public string EstadoAsistencia { get; set; }
            = string.Empty;


        public string HorasTrabajadas =>
            FormatearMinutos(
                MinutosTrabajados);


        public string TiempoFaltante =>
            FormatearMinutos(
                MinutosFaltantes);


        public string TiempoExtra =>
            FormatearMinutos(
                MinutosExtraCalculados);


        public string TardanzaTexto =>
            MinutosTarde <= 0
                ?
                "--"
                :
                $"{MinutosTarde} min";


        private static string FormatearMinutos(
            int minutos)
        {
            if (minutos <= 0)
                return "0:00";

            int horas =
                minutos / 60;

            int minutosRestantes =
                minutos % 60;

            return
                $"{horas}:{minutosRestantes:00}";
        }
    }
}