namespace FerreControl.Application.DTOs.Adelantos;

public class CrearAdelantoAplicacionDto
{
    public int NominaDetalleID { get; set; }

    public decimal Monto { get; set; }

    public string? Observacion { get; set; }
}