namespace FerreControl.Desktop.Models
{
    public class NominaDetalleFila
    {
        public int ID { get; set; }

        public int EmpleadoID { get; set; }

        public string CodigoEmpleado { get; set; }
            = string.Empty;

        public string Empleado { get; set; }
            = string.Empty;


        public decimal SalarioBase { get; set; }

        public decimal Bonificaciones { get; set; }

        public decimal HorasExtra { get; set; }

        public decimal Adelantos { get; set; }

        public decimal OtrosDescuentos { get; set; }

        public decimal TotalPagar { get; set; }

        public decimal TotalPagado { get; set; }

        public decimal Pendiente { get; set; }
    }
}