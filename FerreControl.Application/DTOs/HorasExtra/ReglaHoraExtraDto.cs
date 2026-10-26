namespace FerreControl.Application.DTOs.HorasExtra;

public class ReglaHoraExtraDto
{
    public int ID { get; set; }
    public int NegocioID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public string TipoFraccion { get; set; } = "EXACTO";
    public bool RequiereAprobacion { get; set; }
    public bool CuentaEntradaAnticipada { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }

    public string Descripcion => $"{Nombre} - Q {TarifaHora:N2}/h";
}
