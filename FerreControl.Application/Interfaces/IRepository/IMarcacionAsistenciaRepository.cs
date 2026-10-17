using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Domain.Entities;

namespace FerreControl.Application.Interfaces.IRepository
{
    public interface IMarcacionAsistenciaRepository
    {
        Task<List<MarcacionAsistencia>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha);

        Task<bool> ExisteTipoEnFechaAsync(
            int empleadoId,
            string tipoMarcacion,
            DateTime fecha);

        Task AgregarAsync(
            MarcacionAsistencia marcacion);
    }
}
