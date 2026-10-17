using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.DTOs.EmpleadoHorario;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class EmpleadoHorarioService: IEmpleadoHorarioService
    {
        private readonly IEmpleadoHorarioRepository
            _empleadoHorarioRepository;

        private readonly IEmpleadoRepository
            _empleadoRepository;

        private readonly IHorarioRepository
            _horarioRepository;


        public EmpleadoHorarioService(
            IEmpleadoHorarioRepository empleadoHorarioRepository,
            IEmpleadoRepository empleadoRepository,
            IHorarioRepository horarioRepository)
        {
            _empleadoHorarioRepository =
                empleadoHorarioRepository;

            _empleadoRepository =
                empleadoRepository;

            _horarioRepository =
                horarioRepository;
        }


        public async Task<List<EmpleadoHorarioDto>>
            ObtenerHistorialAsync(int empleadoId)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(empleadoId);

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado indicado no existe.");
            }

            var horarios =
                await _empleadoHorarioRepository
                    .ObtenerHistorialAsync(empleadoId);

            return horarios
                .Select(MapearDto)
                .ToList();
        }


        public async Task<EmpleadoHorarioDto?>
            ObtenerActualAsync(int empleadoId)
        {
            var horario =
                await _empleadoHorarioRepository
                    .ObtenerActualAsync(empleadoId);

            if (horario == null)
            {
                return null;
            }

            return MapearDto(horario);
        }


        public async Task<EmpleadoHorarioDto>
            AsignarHorarioAsync(
                int empleadoId,
                CrearEmpleadoHorarioDto dto)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(empleadoId);

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado indicado no existe.");
            }

            if (!empleado.Estado)
            {
                throw new InvalidOperationException(
                    "El empleado se encuentra inactivo.");
            }


            var horario =
                await _horarioRepository
                    .ObtenerPorIdAsync(dto.HorarioID);

            if (horario == null)
            {
                throw new InvalidOperationException(
                    "El horario indicado no existe.");
            }

            if (!horario.Estado)
            {
                throw new InvalidOperationException(
                    "El horario se encuentra inactivo.");
            }


            if (horario.NegocioID != empleado.NegocioID)
            {
                throw new InvalidOperationException(
                    "El horario no pertenece al mismo negocio del empleado.");
            }


            var fechaInicio =
                dto.FechaInicio.Date;


            var horarioActual =
                await _empleadoHorarioRepository
                    .ObtenerActualAsync(empleadoId);


            if (horarioActual != null)
            {
                if (fechaInicio <=
                    horarioActual.FechaInicio.Date)
                {
                    throw new InvalidOperationException(
                        "La fecha del nuevo horario debe ser posterior a la fecha de inicio del horario actual.");
                }

                horarioActual.FechaFin =
                    fechaInicio.AddDays(-1);

                horarioActual.Estado = false;

                await _empleadoHorarioRepository
                    .ActualizarAsync(horarioActual);
            }


            var nuevoHorario =
                new EmpleadoHorario
                {
                    EmpleadoID = empleadoId,

                    HorarioID = horario.ID,

                    FechaInicio = fechaInicio,

                    FechaFin = null,

                    Estado = true
                };


            await _empleadoHorarioRepository
                .AgregarAsync(nuevoHorario);


            nuevoHorario.Horario = horario;

            return MapearDto(nuevoHorario);
        }


        private static EmpleadoHorarioDto MapearDto(
            EmpleadoHorario empleadoHorario)
        {
            return new EmpleadoHorarioDto
            {
                ID = empleadoHorario.ID,

                EmpleadoID =
                    empleadoHorario.EmpleadoID,

                HorarioID =
                    empleadoHorario.HorarioID,

                HorarioNombre =
                    empleadoHorario.Horario?.Nombre
                    ?? string.Empty,

                FechaInicio =
                    empleadoHorario.FechaInicio,

                FechaFin =
                    empleadoHorario.FechaFin,

                Estado =
                    empleadoHorario.Estado
            };
        }
    }
}
