namespace FerreControl.Domain.Entities;

public class AdelantoEmpleado
{
    public int ID { get; set; }

    public int EmpleadoID { get; set; }

    public long MontoCentavos { get; set; }

    public DateTime FechaAdelanto { get; set; }

    public string MetodoEntrega { get; set; } = string.Empty;

    public string? Referencia { get; set; }

    public string? Observacion { get; set; }

    public string Estado { get; set; } = "PENDIENTE";

    public DateTime FechaCreacion { get; set; } =
        DateTime.Now;

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }


    public Empleado Empleado { get; set; } = null!;

    public ICollection<AdelantoAplicacion> Aplicaciones { get; set; }
        = new List<AdelantoAplicacion>();
}