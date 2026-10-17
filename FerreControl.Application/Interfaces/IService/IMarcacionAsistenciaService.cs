using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.DTOs.MarcacionAsistencia;

namespace FerreControl.Application.Interfaces.IService
{
    public interface IMarcacionAsistenciaService
    {
        Task<MarcacionAsistenciaDto> RegistrarAsync(
            RegistrarMarcacionDto dto);

        Task<List<MarcacionAsistenciaDto>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha);
    }
}