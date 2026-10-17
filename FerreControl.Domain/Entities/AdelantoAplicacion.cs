namespace FerreControl.Domain.Entities;

public class AdelantoAplicacion
{
    public int ID { get; set; }

    public int AdelantoEmpleadoID { get; set; }

    public int NominaDetalleID { get; set; }

    public long MontoCentavos { get; set; }

    public DateTime FechaAplicacion { get; set; }
        = DateTime.Now;

    public string? Observacion { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; }
        = DateTime.Now;

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }


    public AdelantoEmpleado AdelantoEmpleado { get; set; }
        = null!;

    public NominaDetalle NominaDetalle { get; set; }
        = null!;
}