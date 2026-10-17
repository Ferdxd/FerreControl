namespace FerreControl.Application.DTOs.Adelantos;

public class AdelantoEmpleadoDto
{
    public int ID { get; set; }

    public int EmpleadoID { get; set; }

    public decimal Monto { get; set; }

    public decimal TotalAplicado { get; set; }

    public decimal SaldoPendiente { get; set; }

    public DateTime FechaAdelanto { get; set; }

    public string MetodoEntrega { get; set; } =
        string.Empty;

    public string? Referencia { get; set; }

    public string? Observacion { get; set; }

    public string Estado { get; set; } =
        string.Empty;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }
}