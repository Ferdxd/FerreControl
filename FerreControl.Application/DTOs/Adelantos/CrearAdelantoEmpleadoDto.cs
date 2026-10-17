namespace FerreControl.Application.DTOs.Adelantos;

public class CrearAdelantoEmpleadoDto
{
    public decimal Monto { get; set; }

    public DateTime FechaAdelanto { get; set; }

    public string MetodoEntrega { get; set; } =
        string.Empty;

    public string? Referencia { get; set; }

    public string? Observacion { get; set; }
}