using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FerreControl.Application.DTOs.MarcacionAsistencia;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class MarcacionAsistenciaService
        : IMarcacionAsistenciaService
    {
        private readonly IMarcacionAsistenciaRepository
            _marcacionRepository;

        private readonly IEmpleadoCredencialRepository
            _credencialRepository;

        private readonly IEmpleadoRepository
            _empleadoRepository;

        private readonly IEmpleadoHorarioRepository
            _empleadoHorarioRepository;


        public MarcacionAsistenciaService(
            IMarcacionAsistenciaRepository marcacionRepository,
            IEmpleadoCredencialRepository credencialRepository,
            IEmpleadoRepository empleadoRepository,
            IEmpleadoHorarioRepository empleadoHorarioRepository)
        {
            _marcacionRepository =
                marcacionRepository;

            _credencialRepository =
                credencialRepository;

            _empleadoRepository =
                empleadoRepository;

            _empleadoHorarioRepository =
                empleadoHorarioRepository;
        }


        public async Task<MarcacionAsistenciaDto>
            RegistrarAsync(
                RegistrarMarcacionDto dto)
        {
            var tipoCredencial =
                dto.TipoCredencial
                    .Trim()
                    .ToUpperInvariant();

            var codigo =
                dto.Codigo.Trim();

            var tipoMarcacion =
                dto.TipoMarcacion
                    .Trim()
                    .ToUpperInvariant();


            string[] tiposPermitidos =
            {
                "ENTRADA",
                "SALIDA",
                "INICIO_DESCANSO",
                "FIN_DESCANSO"
            };


            if (!tiposPermitidos.Contains(
                tipoMarcacion))
            {
                throw new InvalidOperationException(
                    "Tipo de marcación no válido.");
            }


            var credencial =
                await _credencialRepository
                    .ObtenerActivaPorCodigoAsync(
                        tipoCredencial,
                        codigo);

            if (credencial == null)
            {
                throw new InvalidOperationException(
                    "Credencial no encontrada o inactiva.");
            }


            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        credencial.EmpleadoID);

            if (empleado == null ||
                !empleado.Estado)
            {
                throw new InvalidOperationException(
                    "El empleado se encuentra inactivo.");
            }


            var ahora =
                DateTime.Now;


            var yaExiste =
                await _marcacionRepository
                    .ExisteTipoEnFechaAsync(
                        empleado.ID,
                        tipoMarcacion,
                        ahora);

            if (yaExiste)
            {
                throw new InvalidOperationException(
                    $"El empleado ya registró {tipoMarcacion} el día de hoy.");
            }


            if (tipoMarcacion == "SALIDA")
            {
                var tieneEntrada =
                    await _marcacionRepository
                        .ExisteTipoEnFechaAsync(
                            empleado.ID,
                            "ENTRADA",
                            ahora);

                if (!tieneEntrada)
                {
                    throw new InvalidOperationException(
                        "No puede registrar salida sin haber registrado entrada.");
                }
            }


            if (tipoMarcacion == "FIN_DESCANSO")
            {
                var tieneInicioDescanso =
                    await _marcacionRepository
                        .ExisteTipoEnFechaAsync(
                            empleado.ID,
                            "INICIO_DESCANSO",
                            ahora);

                if (!tieneInicioDescanso)
                {
                    throw new InvalidOperationException(
                        "No puede finalizar el descanso sin haberlo iniciado.");
                }
            }


            var empleadoHorario =
                await _empleadoHorarioRepository
                    .ObtenerVigenteEnFechaAsync(
                        empleado.ID,
                        ahora);


            var marcacion =
                new MarcacionAsistencia
                {
                    EmpleadoID =
                        empleado.ID,

                    EmpleadoCredencialID =
                        credencial.ID,

                    EmpleadoHorarioID =
                        empleadoHorario?.ID,

                    FechaHora =
                        ahora,

                    TipoMarcacion =
                        tipoMarcacion,

                    MetodoMarcacion =
                        credencial.TipoCredencial,

                    Origen =
                        string.IsNullOrWhiteSpace(
                            dto.Origen)
                        ? null
                        : dto.Origen.Trim()
                };


            await _marcacionRepository
                .AgregarAsync(marcacion);


            return MapearDto(marcacion);
        }


        public async Task<List<MarcacionAsistenciaDto>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha)
        {
            var marcaciones =
                await _marcacionRepository
                    .ObtenerPorEmpleadoYFechaAsync(
                        empleadoId,
                        fecha);

            return marcaciones
                .Select(MapearDto)
                .ToList();
        }


        private static MarcacionAsistenciaDto
            MapearDto(
                MarcacionAsistencia marcacion)
        {
            return new MarcacionAsistenciaDto
            {
                ID = marcacion.ID,

                EmpleadoID =
                    marcacion.EmpleadoID,

                EmpleadoHorarioID =
                    marcacion.EmpleadoHorarioID,

                FechaHora =
                    marcacion.FechaHora,

                TipoMarcacion =
                    marcacion.TipoMarcacion,

                MetodoMarcacion =
                    marcacion.MetodoMarcacion,

                Origen =
                    marcacion.Origen
            };
        }
    }
}