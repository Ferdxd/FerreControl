namespace FerreControl.Domain.Entities;

public class RegistroHoraExtra
{
    public int ID { get; set; }
    public int EmpleadoID { get; set; }
    public int? EmpleadoReglaHoraExtraID { get; set; }
    public DateTime Fecha { get; set; }

    public int MinutosCalculados { get; set; }
    public int MinutosAprobados { get; set; }

    public long TarifaHoraCentavos { get; set; }
    public long MontoCentavos { get; set; }

    public string Estado { get; set; } = "PENDIENTE";
    public bool ExcedeLimite12Horas { get; set; }

    public string? Observacion { get; set; }
    public DateTime FechaCalculo { get; set; } = DateTime.Now;
    public DateTime? FechaDecision { get; set; }

    public Empleado Empleado { get; set; } = null!;
    public EmpleadoReglaHoraExtra? EmpleadoReglaHoraExtra { get; set; }
}
