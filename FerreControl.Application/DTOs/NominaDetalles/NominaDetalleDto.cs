namespace FerreControl.Application.DTOs.Nomina
{
    public class NominaDetalleDto
    {
        public int ID { get; set; }

        public int NominaID { get; set; }

        public int EmpleadoID { get; set; }

        public int? SalarioEmpleadoID { get; set; }


        public decimal SalarioBase { get; set; }

        public decimal Bonificaciones { get; set; }

        public decimal HorasExtra { get; set; }

        public decimal Adelantos { get; set; }

        public decimal OtrosDescuentos { get; set; }

        public decimal TotalPagar { get; set; }

        public decimal TotalPagado { get; set; }

        public decimal Pendiente { get; set; }


        public DateTime FechaCreacion { get; set; }
    }
}