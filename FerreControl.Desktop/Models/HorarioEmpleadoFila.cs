namespace FerreControl.Desktop.Models
{
    public class HorarioEmpleadoFila
    {
        public int EmpleadoID
        {
            get;
            set;
        }
        public int HorarioID
        {
            get;
            set;
        }

        public int EmpleadoHorarioID
        {
            get;
            set;
        }

        public string Codigo
        {
            get;
            set;
        }
            = string.Empty;
        public string Empleado
        {
            get;
            set;
        }
            = string.Empty;

        public string Puesto
        {
            get;
            set;
        }
            = string.Empty;

        public string Horario
        {
            get;
            set;
        }
            = string.Empty;
        public DateTime FechaInicio
        {
            get;
            set;
        }
    }
}