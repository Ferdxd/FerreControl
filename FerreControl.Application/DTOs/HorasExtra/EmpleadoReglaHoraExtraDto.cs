namespace FerreControl.Application.DTOs.HorasExtra;

public class EmpleadoReglaHoraExtraDto
{
    public int ID { get; set; }
    public int EmpleadoID { get; set; }
    public int ReglaHoraExtraID { get; set; }
    public string ReglaNombre { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Estado { get; set; }
}
