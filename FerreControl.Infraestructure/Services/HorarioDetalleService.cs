using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerreControl.Application.DTOs.HorarioDetalle;
using FerreControl.Application.Interfaces;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infrastructure.Services
{
    public class HorarioDetalleService
        : IHorarioDetalleService
    {
        private readonly IHorarioDetalleRepository
            _detalleRepository;

        private readonly IHorarioRepository
            _horarioRepository;


        public HorarioDetalleService(
            IHorarioDetalleRepository detalleRepository,
            IHorarioRepository horarioRepository)
        {
            _detalleRepository = detalleRepository;
            _horarioRepository = horarioRepository;
        }


        public async Task<List<HorarioDetalleDto>>
            ObtenerPorHorarioAsync(int horarioId)
        {
            var horario =
                await _horarioRepository
                    .ObtenerPorIdAsync(horarioId);

            if (horario == null)
            {
                throw new InvalidOperationException(
                    "El horario indicado no existe.");
            }


            var detalles =
                await _detalleRepository
                    .ObtenerPorHorarioAsync(horarioId);


            return detalles
                .Select(MapearDto)
                .ToList();
        }


        public async Task<HorarioDetalleDto?>
            ObtenerPorIdAsync(int id)
        {
            var detalle =
                await _detalleRepository
                    .ObtenerPorIdAsync(id);

            if (detalle == null)
            {
                return null;
            }

            return MapearDto(detalle);
        }


        public async Task<HorarioDetalleDto> CrearAsync(
            int horarioId,
            CrearHorarioDetalleDto dto)
        {
            var horario =
                await _horarioRepository
                    .ObtenerPorIdAsync(horarioId);

            if (horario == null)
            {
                throw new InvalidOperationException(
                    "El horario indicado no existe.");
            }


            if (!horario.Estado)
            {
                throw new InvalidOperationException(
                    "No se pueden agregar detalles a un horario inactivo.");
            }


            ValidarDetalle(
                dto.DiaSemana,
                dto.EsLaboral,
                dto.HoraEntrada,
                dto.HoraSalida,
                dto.MinutosDescanso);


            var existeDia =
                await _detalleRepository.ExisteDiaAsync(
                    horarioId,
                    dto.DiaSemana);

            if (existeDia)
            {
                throw new InvalidOperationException(
                    "El día indicado ya se encuentra registrado en este horario.");
            }


            var detalle = new HorarioDetalle
            {
                HorarioID = horarioId,

                DiaSemana = dto.DiaSemana,

                EsLaboral = dto.EsLaboral
            };


            AplicarDatosLaborales(
                detalle,
                dto.EsLaboral,
                dto.HoraEntrada,
                dto.HoraSalida,
                dto.MinutosDescanso);


            await _detalleRepository
                .AgregarAsync(detalle);


            return MapearDto(detalle);
        }


        public async Task<HorarioDetalleDto?>
            ActualizarAsync(
                int id,
                ActualizarHorarioDetalleDto dto)
        {
            var detalle =
                await _detalleRepository
                    .ObtenerPorIdAsync(id);

            if (detalle == null)
            {
                return null;
            }


            ValidarDetalle(
                dto.DiaSemana,
                dto.EsLaboral,
                dto.HoraEntrada,
                dto.HoraSalida,
                dto.MinutosDescanso);


            var existeDia =
                await _detalleRepository.ExisteDiaAsync(
                    detalle.HorarioID,
                    dto.DiaSemana,
                    detalle.ID);

            if (existeDia)
            {
                throw new InvalidOperationException(
                    "El día indicado ya se encuentra registrado en este horario.");
            }


            detalle.DiaSemana = dto.DiaSemana;

            detalle.EsLaboral = dto.EsLaboral;


            AplicarDatosLaborales(
                detalle,
                dto.EsLaboral,
                dto.HoraEntrada,
                dto.HoraSalida,
                dto.MinutosDescanso);


            await _detalleRepository
                .ActualizarAsync(detalle);


            return MapearDto(detalle);
        }


        private static void ValidarDetalle(
            int diaSemana,
            bool esLaboral,
            TimeOnly? horaEntrada,
            TimeOnly? horaSalida,
            int minutosDescanso)
        {
            if (diaSemana < 1 || diaSemana > 7)
            {
                throw new InvalidOperationException(
                    "El día de la semana debe estar entre 1 y 7.");
            }


            if (minutosDescanso < 0)
            {
                throw new InvalidOperationException(
                    "Los minutos de descanso no pueden ser negativos.");
            }


            if (!esLaboral)
            {
                return;
            }


            if (horaEntrada == null)
            {
                throw new InvalidOperationException(
                    "La hora de entrada es obligatoria para un día laboral.");
            }


            if (horaSalida == null)
            {
                throw new InvalidOperationException(
                    "La hora de salida es obligatoria para un día laboral.");
            }


            if (horaSalida <= horaEntrada)
            {
                throw new InvalidOperationException(
                    "La hora de salida debe ser posterior a la hora de entrada.");
            }


            var duracion =
                horaSalida.Value.ToTimeSpan()
                - horaEntrada.Value.ToTimeSpan();

            var minutosJornada =
                duracion.TotalMinutes;


            if (minutosDescanso >= minutosJornada)
            {
                throw new InvalidOperationException(
                    "El descanso no puede ser igual o mayor a la duración de la jornada.");
            }
        }


        private static void AplicarDatosLaborales(
            HorarioDetalle detalle,
            bool esLaboral,
            TimeOnly? horaEntrada,
            TimeOnly? horaSalida,
            int minutosDescanso)
        {
            if (!esLaboral)
            {
                detalle.HoraEntrada = null;
                detalle.HoraSalida = null;
                detalle.MinutosDescanso = 0;

                return;
            }


            detalle.HoraEntrada = horaEntrada;
            detalle.HoraSalida = horaSalida;
            detalle.MinutosDescanso = minutosDescanso;
        }


        private static HorarioDetalleDto MapearDto(
            HorarioDetalle detalle)
        {
            return new HorarioDetalleDto
            {
                ID = detalle.ID,

                HorarioID = detalle.HorarioID,

                DiaSemana = detalle.DiaSemana,

                EsLaboral = detalle.EsLaboral,

                HoraEntrada = detalle.HoraEntrada,

                HoraSalida = detalle.HoraSalida,

                MinutosDescanso =
                    detalle.MinutosDescanso
            };
        }
    }
}