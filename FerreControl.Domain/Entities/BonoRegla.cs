using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Domain.Entities
{
    public class BonoRegla
    {
        public int ID { get; set; }

        public int BonoID { get; set; }

        public string TipoRegla { get; set; } = string.Empty;

        public decimal? Valor { get; set; }

        public string? ValorTexto { get; set; }

        public Bono Bono { get; set; } = null!;
    }
}
