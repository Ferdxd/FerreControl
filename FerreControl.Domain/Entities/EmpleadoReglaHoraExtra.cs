namespace FerreControl.Domain.Entities;

public class EmpleadoReglaHoraExtra
{
    public int ID { get; set; }
    public int EmpleadoID { get; set; }
    public int ReglaHoraExtraID { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public Empleado Empleado { get; set; } = null!;
    public ReglaHoraExtra ReglaHoraExtra { get; set; } = null!;
    public ICollection<RegistroHoraExtra> Registros { get; set; }
        = new List<RegistroHoraExtra>();
}
