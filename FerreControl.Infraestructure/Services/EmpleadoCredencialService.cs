using FerreControl.Application.DTOs.EmpleadoCredencial;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services
{
    public class EmpleadoCredencialService
        : IEmpleadoCredencialService
    {
        private const string TipoCodigoBarras =
            "CODIGO_BARRAS";


        private static readonly string[]
            TiposPermitidos =
        {
            "CODIGO_BARRAS",
            "QR",
            "RFID",
            "HUELLA"
        };


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


        // =====================================================
        // OBTENER CREDENCIALES DEL EMPLEADO
        // =====================================================

        public async Task<List<EmpleadoCredencialDto>>
            ObtenerPorEmpleadoAsync(
                int empleadoId)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        empleadoId);


            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado no existe.");
            }


            var credenciales =
                await _credencialRepository
                    .ObtenerPorEmpleadoAsync(
                        empleadoId);


            return credenciales
                .Select(
                    MapearDto)
                .ToList();
        }


        // =====================================================
        // CREAR CREDENCIAL MANUAL
        // =====================================================

        public async Task<EmpleadoCredencialDto>
            CrearAsync(
                int empleadoId,
                CrearEmpleadoCredencialDto dto)
        {
            var empleado =
                await ObtenerEmpleadoActivoAsync(
                    empleadoId);


            var tipo =
                NormalizarTipo(
                    dto.TipoCredencial);


            var codigo =
                dto.Codigo?
                    .Trim()
                ??
                string.Empty;


            if (!TiposPermitidos.Contains(
                tipo))
            {
                throw new InvalidOperationException(
                    "Tipo de credencial no válido.");
            }


            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                throw new InvalidOperationException(
                    "El código de la credencial es obligatorio.");
            }


            var activa =
                await _credencialRepository
                    .ObtenerActivaPorEmpleadoYTipoAsync(
                        empleado.ID,
                        tipo);


            if (activa != null)
            {
                throw new InvalidOperationException(
                    $"El empleado ya posee una credencial activa de tipo {FormatearTipo(tipo)}.");
            }


            if (await _credencialRepository
                .ExisteCodigoAsync(
                    tipo,
                    codigo))
            {
                throw new InvalidOperationException(
                    "Ese código ya se encuentra registrado.");
            }


            var credencial =
                new EmpleadoCredencial
                {
                    EmpleadoID =
                        empleado.ID,

                    TipoCredencial =
                        tipo,

                    Codigo =
                        codigo,

                    Estado =
                        true,

                    FechaCreacion =
                        DateTime.Now
                };


            await _credencialRepository
                .AgregarAsync(
                    credencial);


            return MapearDto(
                credencial);
        }


        // =====================================================
        // ACTUALIZAR ESTADO
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            ActualizarEstadoAsync(
                int credencialId,
                bool estado)
        {
            var credencial =
                await _credencialRepository
                    .ObtenerPorIdAsync(
                        credencialId);


            if (credencial == null)
                return null;


            if (estado)
            {
                await ObtenerEmpleadoActivoAsync(
                    credencial.EmpleadoID);


                var activa =
                    await _credencialRepository
                        .ObtenerActivaPorEmpleadoYTipoAsync(
                            credencial.EmpleadoID,
                            credencial.TipoCredencial);


                if (activa != null
                    &&
                    activa.ID !=
                    credencial.ID)
                {
                    throw new InvalidOperationException(
                        "Ya existe otra credencial activa de este tipo para el empleado.");
                }
            }


            credencial.Estado =
                estado;


            await _credencialRepository
                .ActualizarAsync(
                    credencial);


            return MapearDto(
                credencial);
        }


        // =====================================================
        // LISTADO DEL MÓDULO DE CARNÉS
        // =====================================================

        public async Task<List<CarnetEmpleadoDto>>
            ObtenerCarnesActivosAsync(
                int negocioId)
        {
            var empleados =
                (await _empleadoRepository
                    .ObtenerTodosAsync())
                .Where(x =>
                    x.NegocioID ==
                    negocioId
                    &&
                    x.Estado)
                .OrderBy(x =>
                    x.Nombres)
                .ThenBy(x =>
                    x.Apellidos)
                .ToList();


            var credenciales =
                await _credencialRepository
                    .ObtenerActivasPorNegocioYTipoAsync(
                        negocioId,
                        TipoCodigoBarras);


            var porEmpleado =
                credenciales
                    .GroupBy(x =>
                        x.EmpleadoID)
                    .ToDictionary(
                        x => x.Key,
                        x => x
                            .OrderByDescending(c =>
                                c.FechaCreacion)
                            .First());


            var resultado =
                new List<CarnetEmpleadoDto>();


            foreach (var empleado
                in empleados)
            {
                porEmpleado.TryGetValue(
                    empleado.ID,
                    out var credencial);


                resultado.Add(
                    new CarnetEmpleadoDto
                    {
                        EmpleadoID =
                            empleado.ID,

                        NegocioID =
                            empleado.NegocioID,

                        CodigoEmpleado =
                            empleado.Codigo,

                        NombreCompleto =
                            $"{empleado.Nombres} {empleado.Apellidos}"
                                .Trim(),

                        Puesto =
                            empleado.Puesto,

                        EmpleadoActivo =
                            empleado.Estado,

                        CredencialID =
                            credencial?.ID,

                        TipoCredencial =
                            credencial?
                                .TipoCredencial,

                        CodigoCredencial =
                            credencial?
                                .Codigo,

                        FechaCredencial =
                            credencial?
                                .FechaCreacion
                    });
            }


            return resultado;
        }


        // =====================================================
        // GENERAR CÓDIGO DE BARRAS
        // =====================================================

        public async Task<EmpleadoCredencialDto>
            GenerarCodigoBarrasAsync(
                int empleadoId)
        {
            var empleado =
                await ObtenerEmpleadoActivoAsync(
                    empleadoId);


            var existente =
                await _credencialRepository
                    .ObtenerActivaPorEmpleadoYTipoAsync(
                        empleado.ID,
                        TipoCodigoBarras);


            if (existente != null)
            {
                throw new InvalidOperationException(
                    "El empleado ya posee un código de barras activo. Utilice Reimprimir o Reemplazar.");
            }


            return await CrearCodigoBarrasInternoAsync(
                empleado,
                null);
        }


        // =====================================================
        // REEMPLAZAR CÓDIGO DE BARRAS
        // =====================================================

        public async Task<EmpleadoCredencialDto>
            ReemplazarCodigoBarrasAsync(
                int empleadoId)
        {
            var empleado =
                await ObtenerEmpleadoActivoAsync(
                    empleadoId);


            var anterior =
                await _credencialRepository
                    .ObtenerActivaPorEmpleadoYTipoAsync(
                        empleado.ID,
                        TipoCodigoBarras);


            return await CrearCodigoBarrasInternoAsync(
                empleado,
                anterior);
        }


        // =====================================================
        // DESACTIVAR CÓDIGO
        // =====================================================

        public async Task<EmpleadoCredencialDto?>
            DesactivarCodigoBarrasAsync(
                int empleadoId)
        {
            var actual =
                await _credencialRepository
                    .ObtenerActivaPorEmpleadoYTipoAsync(
                        empleadoId,
                        TipoCodigoBarras);


            if (actual == null)
                return null;


            actual.Estado =
                false;


            await _credencialRepository
                .ActualizarAsync(
                    actual);


            return MapearDto(
                actual);
        }


        // =====================================================
        // GENERAR FALTANTES
        // =====================================================

        public async Task<List<EmpleadoCredencialDto>>
            GenerarCodigosFaltantesAsync(
                int negocioId)
        {
            var empleados =
                (await _empleadoRepository
                    .ObtenerTodosAsync())
                .Where(x =>
                    x.NegocioID ==
                    negocioId
                    &&
                    x.Estado)
                .ToList();


            var generados =
                new List<EmpleadoCredencialDto>();


            foreach (var empleado
                in empleados)
            {
                var actual =
                    await _credencialRepository
                        .ObtenerActivaPorEmpleadoYTipoAsync(
                            empleado.ID,
                            TipoCodigoBarras);


                if (actual != null)
                    continue;


                var generado =
                    await CrearCodigoBarrasInternoAsync(
                        empleado,
                        null);


                generados.Add(
                    generado);
            }


            return generados;
        }


        // =====================================================
        // CREACIÓN INTERNA
        // =====================================================

        private async Task<EmpleadoCredencialDto>
            CrearCodigoBarrasInternoAsync(
                Empleado empleado,
                EmpleadoCredencial? anterior)
        {
            var codigo =
                await GenerarCodigoUnicoAsync(
                    empleado.ID);


            var nueva =
                new EmpleadoCredencial
                {
                    EmpleadoID =
                        empleado.ID,

                    TipoCredencial =
                        TipoCodigoBarras,

                    Codigo =
                        codigo,

                    Estado =
                        true,

                    FechaCreacion =
                        DateTime.Now
                };


            if (anterior != null)
            {
                await _credencialRepository
                    .ReemplazarAsync(
                        anterior,
                        nueva);
            }
            else
            {
                await _credencialRepository
                    .AgregarAsync(
                        nueva);
            }


            return MapearDto(
                nueva);
        }


        // =====================================================
        // GENERADOR
        // =====================================================

        private async Task<string>
            GenerarCodigoUnicoAsync(
                int empleadoId)
        {
            for (int intento = 0;
                intento < 10;
                intento++)
            {
                var token =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(
                            0,
                            10)
                        .ToUpperInvariant();


                var codigo =
                    $"FC-E{empleadoId}-{token}";


                bool existe =
                    await _credencialRepository
                        .ExisteCodigoAsync(
                            TipoCodigoBarras,
                            codigo);


                if (!existe)
                    return codigo;
            }


            throw new InvalidOperationException(
                "No fue posible generar un código de credencial único.");
        }


        // =====================================================
        // EMPLEADO ACTIVO
        // =====================================================

        private async Task<Empleado>
            ObtenerEmpleadoActivoAsync(
                int empleadoId)
        {
            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        empleadoId);


            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado no existe.");
            }


            if (!empleado.Estado)
            {
                throw new InvalidOperationException(
                    "El empleado se encuentra inactivo.");
            }


            return empleado;
        }


        // =====================================================
        // MAPEO
        // =====================================================

        private static EmpleadoCredencialDto
            MapearDto(
                EmpleadoCredencial credencial)
        {
            return new EmpleadoCredencialDto
            {
                ID =
                    credencial.ID,

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


        private static string
            NormalizarTipo(
                string? tipo)
        {
            return tipo?
                .Trim()
                .ToUpperInvariant()
                ??
                string.Empty;
        }


        private static string
            FormatearTipo(
                string tipo)
        {
            return tipo switch
            {
                "CODIGO_BARRAS" =>
                    "Código de barras",

                "QR" =>
                    "QR",

                "RFID" =>
                    "RFID",

                "HUELLA" =>
                    "Huella",

                _ =>
                    tipo
            };
        }
    }
}