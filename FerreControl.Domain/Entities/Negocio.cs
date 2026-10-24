using System;
using System.Collections.Generic;

namespace FerreControl.Domain.Entities;

public class Negocio
{
    public int ID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<Empleado> Empleados { get; set; }
        = new List<Empleado>();

    public ICollection<Horario> Horarios { get; set; }
        = new List<Horario>();

    public ICollection<Nomina> Nominas { get; set; }
        = new List<Nomina>();

    public ICollection<ReglaHoraExtra> ReglasHoraExtra { get; set; }
        = new List<ReglaHoraExtra>();
}
