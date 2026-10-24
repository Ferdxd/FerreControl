namespace FerreControl.Application.DTOs.HorasExtra;

public class RegistroHoraExtraDto
{
    public int ID { get; set; }
    public int EmpleadoID { get; set; }
    public string CodigoEmpleado { get; set; } = string.Empty;
    public string NombreEmpleado { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string ReglaNombre { get; set; } = string.Empty;

    public int MinutosCalculados { get; set; }
    public int MinutosAprobados { get; set; }

    public decimal TarifaHora { get; set; }
    public decimal MontoEstimado { get; set; }
    public decimal MontoAprobado { get; set; }

    public string Estado { get; set; } = string.Empty;
    public bool ExcedeLimite12Horas { get; set; }
    public string? Observacion { get; set; }

    public DateTime FechaCalculo { get; set; }
    public DateTime? FechaDecision { get; set; }

    public string ExtraCalculadoTexto => FormatearMinutos(MinutosCalculados);
    public string ExtraAprobadoTexto => FormatearMinutos(MinutosAprobados);

    private static string FormatearMinutos(int minutos)
    {
        if (minutos <= 0)
            return "0:00";

        return $"{minutos / 60}:{minutos % 60:00}";
    }
}
