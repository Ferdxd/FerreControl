using FerreControl.Domain.Entities;

public class NominaDia
{
    public int ID { get; set; }

    public int NominaDetalleID { get; set; }

    public DateTime Fecha { get; set; }

    public bool EsLaboral { get; set; }

    public int MinutosProgramados { get; set; }

    public int MinutosTrabajados { get; set; }

    public int MinutosExtra { get; set; }

    public string EstadoAsistencia { get; set; }
        = string.Empty;

    public long MontoBaseDevengadoCentavos { get; set; }

    public long MontoHorasExtraCentavos { get; set; }

    public DateTime FechaCalculo { get; set; }
        = DateTime.Now;

    public NominaDetalle NominaDetalle { get; set; }
        = null!;
}