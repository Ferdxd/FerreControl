using FerreControl.Domain.Entities;

public class ConfiguracionHoraExtraEmpleado
{
    public int ID { get; set; }

    public int EmpleadoID { get; set; }

    public bool AplicaHorasExtra { get; set; }

    public long TarifaHoraCentavos { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; }
        = DateTime.Now;

    public Empleado Empleado { get; set; } = null!;
}