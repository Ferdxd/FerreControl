namespace FerreControl.Application.DTOs.Adelantos;

public class AdelantoAplicacionDto
{
    public int ID { get; set; }

    public int AdelantoEmpleadoID { get; set; }

    public int NominaDetalleID { get; set; }

    public decimal Monto { get; set; }

    public DateTime FechaAplicacion { get; set; }

    public string? Observacion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }
}