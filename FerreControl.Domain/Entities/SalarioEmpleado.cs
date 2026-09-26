using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities;

public class SalarioEmpleado
{
    public int ID { get; set; }

    public int EmpleadoID { get; set; }

    public long MontoCentavos { get; set; }

    public string Periodicidad { get; set; } = "Semanal";

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool Estado { get; set; } = true;

    public Empleado Empleado { get; set; } = null!;
}

