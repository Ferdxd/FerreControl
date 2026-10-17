namespace FerreControl.Application.DTOs.Nomina
{
    public class CrearNominaDetalleDto
    {
        public int EmpleadoID { get; set; }

        public decimal Bonificaciones { get; set; } = 0;

        public decimal HorasExtra { get; set; } = 0;

        public decimal OtrosDescuentos { get; set; } = 0;
    }
}