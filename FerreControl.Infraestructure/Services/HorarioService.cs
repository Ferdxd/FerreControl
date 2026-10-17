using FerreControl.Application.DTOs.Horarios;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Infraestructure.Services
{
    public class HorarioService : IHorarioService
    {
        private readonly IHorarioRepository _horarioRepository;
        private readonly INegocioRepository _negocioRepository;
        public HorarioService(IHorarioRepository horarioRepository, INegocioRepository negocioRepository)
        {
            _horarioRepository = horarioRepository;
            _negocioRepository = negocioRepository;
        }
        public async Task<HorarioDto> ActualizarHorarioAsync(int id, ActualizarHorarioDto actualizarHorarioDto)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if (horario == null)
            {
                throw new InvalidOperationException("El Id no es valido");
            }
            horario.Nombre = actualizarHorarioDto.Nombre;
            horario.ToleranciaMinutos = actualizarHorarioDto.ToleranciaMinutos;
            horario.Estado = actualizarHorarioDto.Estado;

            await _horarioRepository.ActualizarAsync(horario);

            return new HorarioDto
            {
                ID = horario.ID,
                NegocioID = horario.NegocioID,
                Nombre = horario.Nombre,
                ToleranciaMinutos = horario.ToleranciaMinutos,
                Estado = horario.Estado,
                FechaCreacion = horario.FechaCreacion
            };

        }

        public async Task<HorarioDto> CrearHorarioAsync(CrearHorarioDto crearHorarioDto)
        {
            var negocio = await _negocioRepository.ObtenerPorIdAsync(crearHorarioDto.NegocioID);
            if (negocio == null)
            {
                throw new InvalidOperationException("El Id no es valido");
            }
            if (negocio.Estado == false)
            {
                throw new InvalidOperationException("Negocio Inactivo, no se puede crear horario");
            }
            if(crearHorarioDto.ToleranciaMinutos < 0)
            {
                throw new InvalidOperationException("La tolerancia no puede ser negativa");
            }
            var horario = new Horario
            {
                NegocioID = crearHorarioDto.NegocioID,
                Nombre = crearHorarioDto.Nombre,
                ToleranciaMinutos = crearHorarioDto.ToleranciaMinutos,
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            await _horarioRepository.AgregarAsync(horario);
            return new HorarioDto
            {
                ID = horario.ID,
                NegocioID = horario.NegocioID,
                Nombre = horario.Nombre,
                ToleranciaMinutos = horario.ToleranciaMinutos,
                Estado = horario.Estado,
                FechaCreacion = horario.FechaCreacion
            };

        }
        public async Task<HorarioDto?> ObtenerHorarioPorIdAsync(int id)
        {
            var horario = await _horarioRepository.ObtenerPorIdAsync(id);
            if(horario == null)
            {
                return null;
            }

            return new HorarioDto
            {
                ID=horario.ID,
                NegocioID=horario.NegocioID,
                Nombre=horario.Nombre,
                ToleranciaMinutos = horario.ToleranciaMinutos,
                Estado = horario.Estado,
                FechaCreacion = horario.FechaCreacion
            };


        }

        public async Task<IEnumerable<HorarioDto>> ObtenerHorariosPorNegocioIdAsync(int negocioId)
        {
            var negocio = await _negocioRepository.ObtenerPorIdAsync(negocioId);
            if (negocio==null)
            {
                throw new InvalidOperationException("Id de Negocio Invalida");
            }
            var horarios= await _horarioRepository.ObtenerHorarioPorNegocioAsync(negocioId);

            return horarios.Select(x => new HorarioDto
            {
                ID=x.ID,
                NegocioID = x.NegocioID,
                Nombre=x.Nombre,
                ToleranciaMinutos=x.ToleranciaMinutos,
                Estado=x.Estado,
                FechaCreacion=x.FechaCreacion
            });

        }
    }
}
