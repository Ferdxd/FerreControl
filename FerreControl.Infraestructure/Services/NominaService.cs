using FerreControl.Application.DTOs.Nomina;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class NominaService : INominaService
    {
        private readonly INominaRepository _nominaRepository;

        public NominaService(
            INominaRepository nominaRepository)
        {
            _nominaRepository = nominaRepository;
        }


        public async Task<List<NominaDto>>
            ObtenerPorNegocioAsync(int negocioId)
        {
            var nominas =
                await _nominaRepository
                    .ObtenerPorNegocioAsync(negocioId);

            return nominas
                .Select(MapearDto)
                .ToList();
        }


        public async Task<NominaDto?>
            ObtenerPorIdAsync(int id)
        {
            var nomina =
                await _nominaRepository
                    .ObtenerPorIdAsync(id);

            return nomina == null
                ? null
                : MapearDto(nomina);
        }


        public async Task<NominaDto> CrearAsync(
            CrearNominaDto dto)
        {
            if (dto.FechaInicioPeriodo >
                dto.FechaFinPeriodo)
            {
                throw new InvalidOperationException(
                    "La fecha inicial no puede ser mayor a la fecha final.");
            }


            var nomina = new Nomina
            {
                NegocioID = dto.NegocioID,

                FechaInicioPeriodo =
                    dto.FechaInicioPeriodo.Date,

                FechaFinPeriodo =
                    dto.FechaFinPeriodo.Date,

                FechaGeneracion =
                    DateTime.Now,

                Estado =
                    "BORRADOR"
            };


            await _nominaRepository
                .AgregarAsync(nomina);


            return MapearDto(nomina);
        }


        public async Task<NominaDto>
            ActualizarEstadoAsync(
                int id,
                ActualizarEstadoNominaDto dto)
        {
            var nomina =
                await _nominaRepository
                    .ObtenerPorIdAsync(id);

            if (nomina == null)
            {
                throw new InvalidOperationException(
                    "Nómina no encontrada.");
            }


            var estado =
                dto.Estado
                    .Trim()
                    .ToUpper();


            if (estado != "BORRADOR" &&
                estado != "GENERADA" &&
                estado != "CERRADA")
            {
                throw new InvalidOperationException(
                    "Estado de nómina no válido.");
            }


            nomina.Estado = estado;


            await _nominaRepository
                .ActualizarAsync(nomina);


            return MapearDto(nomina);
        }


        private static NominaDto MapearDto(
            Nomina nomina)
        {
            return new NominaDto
            {
                ID = nomina.ID,

                NegocioID =
                    nomina.NegocioID,

                FechaInicioPeriodo =
                    nomina.FechaInicioPeriodo,

                FechaFinPeriodo =
                    nomina.FechaFinPeriodo,

                FechaGeneracion =
                    nomina.FechaGeneracion,

                Estado =
                    nomina.Estado
            };
        }
    }
}