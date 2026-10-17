
using FerreControl.Application.DTOs.EmpleadoCredencial;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class EmpleadoCredencialService
        : IEmpleadoCredencialService
    {
        private readonly IEmpleadoCredencialRepository
            _credencialRepository;

        private readonly IEmpleadoRepository
            _empleadoRepository;


        public EmpleadoCredencialService(
            IEmpleadoCredencialRepository credencialRepository,
            IEmpleadoRepository empleadoRepository)
        {
            _credencialRepository =
                credencialRepository;

            _empleadoRepository =
                empleadoRepository;
        }


        public async Task<List<EmpleadoCredencialDto>>
            ObtenerPorEmpleadoAsync(int empleadoId)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(empleadoId);

            if (empleado == null)
                throw new InvalidOperationException(
                    "El empleado no existe.");

            var credenciales =
                await _credencialRepository
                    .ObtenerPorEmpleadoAsync(empleadoId);

            return credenciales
                .Select(MapearDto)
                .ToList();
        }


        public async Task<EmpleadoCredencialDto>
            CrearAsync(
                int empleadoId,
                CrearEmpleadoCredencialDto dto)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(empleadoId);

            if (empleado == null)
                throw new InvalidOperationException(
                    "El empleado no existe.");

            if (!empleado.Estado)
                throw new InvalidOperationException(
                    "El empleado está inactivo.");


            var tipo =
                dto.TipoCredencial
                    .Trim()
                    .ToUpperInvariant();

            var codigo =
                dto.Codigo.Trim();


            string[] tiposPermitidos =
            {
                "CODIGO_BARRAS",
                "QR",
                "RFID",
                "HUELLA"
            };


            if (!tiposPermitidos.Contains(tipo))
            {
                throw new InvalidOperationException(
                    "Tipo de credencial no válido.");
            }


            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new InvalidOperationException(
                    "El código es obligatorio.");
            }


            if (await _credencialRepository
                .ExisteCodigoAsync(tipo, codigo))
            {
                throw new InvalidOperationException(
                    "La credencial ya se encuentra registrada.");
            }


            var credencial =
                new EmpleadoCredencial
                {
                    EmpleadoID = empleadoId,

                    TipoCredencial = tipo,

                    Codigo = codigo,

                    Estado = true,

                    FechaCreacion = DateTime.Now
                };


            await _credencialRepository
                .AgregarAsync(credencial);


            return MapearDto(credencial);
        }


        public async Task<EmpleadoCredencialDto?>
            ActualizarEstadoAsync(
                int credencialId,
                bool estado)
        {
            var credencial =
                await _credencialRepository
                    .ObtenerPorIdAsync(credencialId);

            if (credencial == null)
                return null;


            credencial.Estado = estado;

            await _credencialRepository
                .ActualizarAsync(credencial);


            return MapearDto(credencial);
        }


        private static EmpleadoCredencialDto MapearDto(
            EmpleadoCredencial credencial)
        {
            return new EmpleadoCredencialDto
            {
                ID = credencial.ID,

                EmpleadoID =
                    credencial.EmpleadoID,

                TipoCredencial =
                    credencial.TipoCredencial,

                Codigo =
                    credencial.Codigo,

                Estado =
                    credencial.Estado
            };
        }
    }
}