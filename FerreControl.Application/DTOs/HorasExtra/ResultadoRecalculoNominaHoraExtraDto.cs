namespace FerreControl.Application.DTOs.HorasExtra;

public class ResultadoRecalculoNominaHoraExtraDto
{
    public int NominaID { get; set; }
    public int DetallesActualizados { get; set; }
    public int MinutosExtraAprobados { get; set; }
    public decimal MontoHorasExtra { get; set; }
}
