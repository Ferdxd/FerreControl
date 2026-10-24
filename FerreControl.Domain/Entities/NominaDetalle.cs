

namespace FerreControl.Domain.Entities;
public class NominaDetalle
{
    public int ID { get; set; }

    public int NominaID { get; set; }

    public int EmpleadoID { get; set; }

    public int? SalarioEmpleadoID { get; set; }


    public long SalarioBaseCentavos { get; set; }

    public long SalarioDevengadoCentavos { get; set; }

    public long BonificacionesCentavos { get; set; }

    public long HorasExtraCentavos { get; set; }

    public long AdelantosCentavos { get; set; }

    public long OtrosDescuentosCentavos { get; set; }

    public long TotalPagarCentavos { get; set; }


    public int DiasProgramados { get; set; }

    public int DiasTrabajados { get; set; }

    public int MinutosTrabajados { get; set; }

    public int MinutosExtra { get; set; }


    public DateTime FechaCreacion { get; set; }

    public Nomina Nomina { get; set; } = null!;

    public Empleado Empleado { get; set; } = null!;

    public SalarioEmpleado? SalarioEmpleado { get; set; }

    public ICollection<PagoNomina> Pagos { get; set; }
        = new List<PagoNomina>();

    public ICollection<AdelantoAplicacion> AdelantosAplicados { get; set; }
        = new List<AdelantoAplicacion>();
}