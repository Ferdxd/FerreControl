using FerreControl.Application.DTOs.Salarios;
using FerreControl.Application.Interfaces;
using FerreControl.Domain.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerreControl.Infraestructure.Services
{
    public class SalarioEmpleadoService: ISalarioEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly ISalarioEmpleadoRepository _salarioRepository;

        public SalarioEmpleadoService(IEmpleadoRepository empleadoRepository, ISalarioEmpleadoRepository salarioRepository)
        {
            _empleadoRepository = empleadoRepository;
            _salarioRepository = salarioRepository;
        }

        public async Task<SalarioEmpleadoDto?> ObtenerActualAsync(int idEmpleado)
        {
            var salario = await _salarioRepository.ObtenerActualAsync(idEmpleado);
            if (salario == null)
            {
                throw new InvalidOperationException("No existen registros");
            }

            return new SalarioEmpleadoDto
            {
                ID = salario.ID,
                EmpleadoID = salario.EmpleadoID,
                Monto = salario.MontoCentavos/100m,
                Periodicidad = salario.Periodicidad,
                FechaInicio = salario.FechaInicio,
                FechaFin = salario.FechaFin,
                Estado = salario.Estado
            };
        }

        public async Task<List<SalarioEmpleadoDto>> ObtenerHistorialAsync(int idEmpleado)
        {
            var empleado = _empleadoRepository.ObtenerPorIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException("El empleado no existe");
            }

            var salario = await _salarioRepository.ObtenerHistorialAsync(idEmpleado);

            return salario.Select(x => new SalarioEmpleadoDto
            {
                ID=x.ID,
                EmpleadoID=x.EmpleadoID,
                Monto=x.MontoCentavos/100m,
                Periodicidad=x.Periodicidad,
                FechaInicio=x.FechaInicio,
                FechaFin=x.FechaFin,
                Estado=x.Estado
            }).ToList();

           
            
        }

        public async Task<SalarioEmpleadoDto> RegistrarSalarioAsync(int idEmpleado, CrearSalarioEmpleadoDto dto)
        {
            var empleado = await _empleadoRepository.ObtenerPorIdAsync(idEmpleado);
            
            if (empleado == null)
            {
                throw new InvalidOperationException("El empleado no existe");
            }
            if (empleado.Estado==false)
            {
                throw new InvalidOperationException("El empleado esta inactivo");
            }
            if (dto.Monto <= 0)
            {
                throw new InvalidOperationException("El monto debe ser mayor a cero");
            }
            if (string.IsNullOrWhiteSpace(dto.Periodicidad))
            {
                throw new InvalidOperationException("No tiene Periodicidad");
            }

            var salario = new SalarioEmpleado()
            {
                EmpleadoID = idEmpleado,
                MontoCentavos = (long)Math.Round(dto.Monto * 100m, MidpointRounding.AwayFromZero),
                Periodicidad = dto.Periodicidad.Trim().ToUpperInvariant(),
                FechaInicio=dto.FechaInicio,
                FechaFin=null,
                Estado = true
            };

       

            var tieneSalario = await _salarioRepository.ObtenerActualAsync(idEmpleado);
            if (tieneSalario != null)
            {
                //validar fecha
                if (dto.FechaInicio > tieneSalario.FechaInicio)
                {
                    tieneSalario.FechaFin =dto.FechaInicio.AddDays(-1);

                    tieneSalario.Estado = false;

                    await _salarioRepository
                        .ActualizarAsync(tieneSalario);
                }
                else {
                    throw new InvalidOperationException("El salario actual tiene una fecha mas reciente que el que se intenta registrar");
                }
                
            }
            await _salarioRepository.AgregarAsync(salario);



            return new SalarioEmpleadoDto()
            {
                ID=salario.ID,
                EmpleadoID=salario.EmpleadoID,
                Monto=salario.MontoCentavos/100m,
                Periodicidad=salario.Periodicidad,
                FechaInicio=salario.FechaInicio,
                FechaFin=salario.FechaFin,
                Estado=salario.Estado
            };

  
        }
    }
}
