namespace FerreControl.Application.DTOs.HorasExtra;

public class CrearReglaHoraExtraDto
{
    public int NegocioID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public bool RequiereAprobacion { get; set; } = true;
    public bool CuentaEntradaAnticipada { get; set; } = false;
}
