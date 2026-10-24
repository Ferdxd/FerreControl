namespace FerreControl.Application.DTOs.MarcacionAsistencia
{
    public class RegistrarMarcacionAutomaticaDto
    {
        public string TipoCredencial { get; set; }
            = string.Empty;

        public string Codigo { get; set; }
            = string.Empty;

        public string? Origen { get; set; }
    }
}