using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities;

public class Empleado
{
    public int ID { get; set; }

    public int NegocioID { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? Puesto { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaBaja { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public Negocio Negocio { get; set; } = null!;

    public ICollection<SalarioEmpleado> Salarios { get; set; }
        = new List<SalarioEmpleado>();
}