using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Application.DTOs.HorarioDetalle
{
    public class HorarioDetalleDto
    {
        public int ID { get; set; }

        public int HorarioID { get; set; }

        public int DiaSemana { get; set; }

        public bool EsLaboral { get; set; }

        public TimeOnly? HoraEntrada { get; set; }

        public TimeOnly? HoraSalida { get; set; }

        public int MinutosDescanso { get; set; }

    }
}
