using FerreControl.Application.DTOs.Negocios;
using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Infraestructure.Services
{
    public class NegocioService : INegocioService
    {
        private readonly INegocioRepository _negocioRespository;

        public NegocioService(INegocioRepository negocioRespository)
        {
            _negocioRespository = negocioRespository;
        }

        public async Task<NegocioDto> ActualizarNegocio(int id, ActualizarNegocioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new InvalidOperationException("El nombre del negocio es obligatorio");
            }
            if (string.IsNullOrWhiteSpace(dto.Tipo))
            {
                throw new InvalidOperationException("El tipo de negocio es obligatorio");
            }

            var negocio = await _negocioRespository.ObtenerPorIdAsync(id);
            if (negocio == null)
            {
                return null;
            }

            negocio.Nombre = dto.Nombre.Trim();
            negocio.Tipo = dto.Tipo.Trim();
            negocio.Estado = dto.Estado;

            await _negocioRespository.ActualizarAsync(negocio);

            return new NegocioDto()
            {
                ID=negocio.ID,
                Nombre=negocio.Nombre,
                Tipo=negocio.Tipo,
                Estado=negocio.Estado

            };
        }

        public async Task<NegocioDto> CrearNegocio(CrearNegocioDto dto)
        {
            var negocio = new Negocio
            {
                Nombre=dto.Nombre,
                Tipo=dto.Tipo,
                Estado=true,
                FechaCreacion=DateTime.Now
                
            };
            await _negocioRespository.AgregarAsync(negocio);

            return new NegocioDto
            {
                ID=negocio.ID,
                Nombre=negocio.Nombre,
                Tipo=negocio.Tipo,
                Estado=negocio.Estado
            };
        }

        public async Task<NegocioDto?> ObtenerPorIdAsync(int id)
        {
            var negocio = await _negocioRespository.ObtenerPorIdAsync(id);
            if (negocio == null)
            {
                return null;
            }
            return new NegocioDto
            {
                ID = negocio.ID,
                Nombre= negocio.Nombre,
                Tipo=negocio.Tipo,
                Estado=negocio.Estado
            };
        }

        public async Task<List<NegocioDto>> ObtenerTodosAsync()
        {
            var negocio = await _negocioRespository.ObtenerTodosAsync();

            return negocio.Select(x => new NegocioDto
            {
                ID = x.ID,
                Nombre = x.Nombre,
                Tipo = x.Tipo,
                Estado = x.Estado
            }).ToList();
        }
    }
}
