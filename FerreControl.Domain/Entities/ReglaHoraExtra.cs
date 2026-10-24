namespace FerreControl.Domain.Entities;

public class ReglaHoraExtra
{
    public int ID { get; set; }
    public int NegocioID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public long TarifaHoraCentavos { get; set; }
    public string TipoFraccion { get; set; } = "EXACTO";
    public bool RequiereAprobacion { get; set; } = true;
    public bool CuentaEntradaAnticipada { get; set; } = false;
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public Negocio Negocio { get; set; } = null!;
    public ICollection<EmpleadoReglaHoraExtra> Asignaciones { get; set; }
        = new List<EmpleadoReglaHoraExtra>();
}
