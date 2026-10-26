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

        private readonly IHorarioDetalleRepository
            _horarioDetalleRepository;


        private static readonly string[]
            TiposPermitidos =
        {
            "ENTRADA",
            "INICIO_DESCANSO",
            "FIN_DESCANSO",
            "SALIDA"
        };


        public MarcacionAsistenciaService(
            IMarcacionAsistenciaRepository marcacionRepository,
            IEmpleadoCredencialRepository credencialRepository,
            IEmpleadoRepository empleadoRepository,
            IEmpleadoHorarioRepository empleadoHorarioRepository,
            IHorarioDetalleRepository horarioDetalleRepository)
        {
            _marcacionRepository =
                marcacionRepository;

            _credencialRepository =
                credencialRepository;

            _empleadoRepository =
                empleadoRepository;

            _empleadoHorarioRepository =
                empleadoHorarioRepository;

            _horarioDetalleRepository =
                horarioDetalleRepository;
        }


        // =====================================================
        // REGISTRO EXPLÍCITO
        // =====================================================

        public async Task<MarcacionAsistenciaDto>
            RegistrarAsync(
                RegistrarMarcacionDto dto)
        {
            if (dto == null)
            {
                throw new InvalidOperationException(
                    "Los datos de marcación son obligatorios.");
            }


            var tipoMarcacion =
                NormalizarTipoMarcacion(
                    dto.TipoMarcacion);


            var contexto =
                await ObtenerContextoCredencialAsync(
                    dto.TipoCredencial,
                    dto.Codigo);


            var ahora =
                DateTime.Now;


            var marcaciones =
                await _marcacionRepository
                    .ObtenerPorEmpleadoYFechaAsync(
                        contexto.Empleado.ID,
                        ahora);


            var horario =
                await ObtenerHorarioDiaAsync(
                    contexto.Empleado.ID,
                    ahora);


            bool utilizaDescanso =
                horario.Detalle?.EsLaboral == true
                &&
                horario.Detalle.MinutosDescanso > 0;


            var siguiente =
                DeterminarSiguienteMarcacion(
                    marcaciones,
                    utilizaDescanso);


            if (!string.Equals(
                tipoMarcacion,
                siguiente,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"La siguiente marcación permitida es {FormatearTipo(siguiente)}.");
            }


            var marcacion =
                CrearMarcacionCredencial(
                    contexto.Empleado,
                    contexto.Credencial,
                    horario.Asignacion,
                    ahora,
                    tipoMarcacion,
                    dto.Origen);


            await _marcacionRepository
                .AgregarAsync(
                    marcacion);


            return MapearDto(
                marcacion,
                contexto.Empleado);
        }


        // =====================================================
        // MARCAJE AUTOMÁTICO
        // =====================================================

        public async Task<MarcacionAsistenciaDto>
            RegistrarAutomaticoAsync(
                RegistrarMarcacionAutomaticaDto dto)
        {
            if (dto == null)
            {
                throw new InvalidOperationException(
                    "Los datos de marcación son obligatorios.");
            }


            var contexto =
                await ObtenerContextoCredencialAsync(
                    dto.TipoCredencial,
                    dto.Codigo);


            var ahora =
                DateTime.Now;


            var marcaciones =
                await _marcacionRepository
                    .ObtenerPorEmpleadoYFechaAsync(
                        contexto.Empleado.ID,
                        ahora);


            var horario =
                await ObtenerHorarioDiaAsync(
                    contexto.Empleado.ID,
                    ahora);


            bool utilizaDescanso =
                horario.Detalle?.EsLaboral == true
                &&
                horario.Detalle.MinutosDescanso > 0;


            string tipoMarcacion =
                DeterminarSiguienteMarcacion(
                    marcaciones,
                    utilizaDescanso);


            var marcacion =
                CrearMarcacionCredencial(
                    contexto.Empleado,
                    contexto.Credencial,
                    horario.Asignacion,
                    ahora,
                    tipoMarcacion,
                    dto.Origen);


            await _marcacionRepository
                .AgregarAsync(
                    marcacion);


            return MapearDto(
                marcacion,
                contexto.Empleado);
        }


        // =====================================================
        // REGISTRO MANUAL
        // =====================================================

        public async Task<List<MarcacionAsistenciaDto>>
            RegistrarManualAsync(
                RegistrarAsistenciaManualDto dto)
        {
            if (dto == null)
            {
                throw new InvalidOperationException(
                    "Los datos de asistencia son obligatorios.");
            }


            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        dto.EmpleadoID);


            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado no existe.");
            }


            var fecha =
                dto.Fecha.Date;


            if (fecha >
                DateTime.Today)
            {
                throw new InvalidOperationException(
                    "No puede registrar asistencia en una fecha futura.");
            }


            if (empleado.FechaIngreso.HasValue
                &&
                fecha <
                empleado.FechaIngreso.Value.Date)
            {
                throw new InvalidOperationException(
                    "La fecha es anterior al ingreso del empleado.");
            }


            if (empleado.FechaBaja.HasValue
                &&
                fecha >
                empleado.FechaBaja.Value.Date)
            {
                throw new InvalidOperationException(
                    "La fecha es posterior a la baja del empleado.");
            }


            var existentes =
                await _marcacionRepository
                    .ObtenerPorEmpleadoYFechaAsync(
                        empleado.ID,
                        fecha);


            var horario =
                await ObtenerHorarioDiaAsync(
                    empleado.ID,
                    fecha);


            var nuevas =
                new List<MarcacionAsistencia>();


            AgregarManualSiCorresponde(
                nuevas,
                existentes,
                empleado,
                horario.Asignacion,
                fecha,
                dto.HoraEntrada,
                "ENTRADA",
                dto.Observacion);


            AgregarManualSiCorresponde(
                nuevas,
                existentes,
                empleado,
                horario.Asignacion,
                fecha,
                dto.HoraInicioDescanso,
                "INICIO_DESCANSO",
                dto.Observacion);


            AgregarManualSiCorresponde(
                nuevas,
                existentes,
                empleado,
                horario.Asignacion,
                fecha,
                dto.HoraFinDescanso,
                "FIN_DESCANSO",
                dto.Observacion);


            AgregarManualSiCorresponde(
                nuevas,
                existentes,
                empleado,
                horario.Asignacion,
                fecha,
                dto.HoraSalida,
                "SALIDA",
                dto.Observacion);


            if (nuevas.Count == 0)
            {
                throw new InvalidOperationException(
                    "No hay nuevas marcaciones para registrar.");
            }


            var todas =
                existentes
                    .Concat(nuevas)
                    .OrderBy(x =>
                        x.FechaHora)
                    .ToList();


            ValidarJornadaManual(
                todas);


            await _marcacionRepository
                .AgregarRangoAsync(
                    nuevas);


            return nuevas
                .OrderBy(x =>
                    x.FechaHora)
                .Select(x =>
                    MapearDto(
                        x,
                        empleado))
                .ToList();
        }


        // =====================================================
        // MARCACIONES POR EMPLEADO
        // =====================================================

        public async Task<List<MarcacionAsistenciaDto>>
            ObtenerPorEmpleadoYFechaAsync(
                int empleadoId,
                DateTime fecha)
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


            var marcaciones =
                await _marcacionRepository
                    .ObtenerPorEmpleadoYFechaAsync(
                        empleadoId,
                        fecha);


            return marcaciones
                .Select(x =>
                    MapearDto(
                        x,
                        empleado))
                .ToList();
        }


        // =====================================================
        // RESUMEN DIARIO DE ASISTENCIA
        // =====================================================

        public async Task<List<AsistenciaDiariaDto>>
            ObtenerResumenDiarioAsync(
                int negocioId,
                DateTime fecha)
        {
            var dia =
                fecha.Date;


            var empleados =
                (await _empleadoRepository
                    .ObtenerTodosAsync())
                .Where(x =>
                    x.NegocioID == negocioId
                    &&
                    (
                        !x.FechaIngreso.HasValue
                        ||
                        x.FechaIngreso.Value.Date <= dia
                    )
                    &&
                    (
                        !x.FechaBaja.HasValue
                        ||
                        x.FechaBaja.Value.Date >= dia
                    ))
                .OrderBy(x =>
                    x.Nombres)
                .ThenBy(x =>
                    x.Apellidos)
                .ToList();


            var marcacionesDia =
                await _marcacionRepository
                    .ObtenerPorFechaAsync(
                        dia);


            var resultado =
                new List<AsistenciaDiariaDto>();


            foreach (var empleado in empleados)
            {
                var horario =
                    await ObtenerHorarioDiaAsync(
                        empleado.ID,
                        dia);


                var marcacionesEmpleado =
                    marcacionesDia
                        .Where(x =>
                            x.EmpleadoID ==
                            empleado.ID)
                        .OrderBy(x =>
                            x.FechaHora)
                        .ToList();


                resultado.Add(
                    ConstruirResumenDiario(
                        empleado,
                        horario.Asignacion,
                        horario.Detalle,
                        marcacionesEmpleado,
                        dia));
            }


            return resultado;
        }


        // =====================================================
        // CONTEXTO DE CREDENCIAL
        // =====================================================

        private async Task<(
            EmpleadoCredencial Credencial,
            Empleado Empleado)>
            ObtenerContextoCredencialAsync(
                string tipoCredencial,
                string codigo)
        {
            if (string.IsNullOrWhiteSpace(
                tipoCredencial))
            {
                throw new InvalidOperationException(
                    "Debe indicar el tipo de credencial.");
            }


            if (string.IsNullOrWhiteSpace(
                codigo))
            {
                throw new InvalidOperationException(
                    "No se recibió un código de credencial.");
            }


            var tipo =
                tipoCredencial
                    .Trim()
                    .ToUpperInvariant();


            var codigoLimpio =
                codigo.Trim();


            var credencial =
                await _credencialRepository
                    .ObtenerActivaPorCodigoAsync(
                        tipo,
                        codigoLimpio);


            if (credencial == null)
            {
                throw new InvalidOperationException(
                    "Credencial no encontrada o inactiva.");
            }


            var empleado =
                await _empleadoRepository
                    .ObtenerPorIdAsync(
                        credencial.EmpleadoID);


            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado asociado a la credencial no existe.");
            }


            if (!empleado.Estado)
            {
                throw new InvalidOperationException(
                    "El empleado se encuentra inactivo.");
            }


            return (
                credencial,
                empleado);
        }


        // =====================================================
        // HORARIO DEL DÍA
        // =====================================================

        private async Task<(
            EmpleadoHorario? Asignacion,
            HorarioDetalle? Detalle)>
            ObtenerHorarioDiaAsync(
                int empleadoId,
                DateTime fecha)
        {
            var asignacion =
                await _empleadoHorarioRepository
                    .ObtenerVigenteEnFechaAsync(
                        empleadoId,
                        fecha);


            if (asignacion == null)
            {
                return (
                    null,
                    null);
            }


            var detalles =
                await _horarioDetalleRepository
                    .ObtenerPorHorarioAsync(
                        asignacion.HorarioID);


            int diaSemana =
                ConvertirDiaSemana(
                    fecha);


            var detalle =
                detalles.FirstOrDefault(x =>
                    x.DiaSemana ==
                    diaSemana);


            return (
                asignacion,
                detalle);
        }


        // =====================================================
        // SIGUIENTE MARCACIÓN AUTOMÁTICA
        // =====================================================

        private static string
            DeterminarSiguienteMarcacion(
                List<MarcacionAsistencia> marcaciones,
                bool utilizaDescanso)
        {
            bool tieneEntrada =
                marcaciones.Any(x =>
                    x.TipoMarcacion ==
                    "ENTRADA");


            bool tieneInicioDescanso =
                marcaciones.Any(x =>
                    x.TipoMarcacion ==
                    "INICIO_DESCANSO");


            bool tieneFinDescanso =
                marcaciones.Any(x =>
                    x.TipoMarcacion ==
                    "FIN_DESCANSO");


            bool tieneSalida =
                marcaciones.Any(x =>
                    x.TipoMarcacion ==
                    "SALIDA");


            if (tieneSalida)
            {
                throw new InvalidOperationException(
                    "La jornada del empleado ya fue finalizada.");
            }


            if (!tieneEntrada)
            {
                if (tieneInicioDescanso
                    ||
                    tieneFinDescanso)
                {
                    throw new InvalidOperationException(
                        "La jornada tiene marcaciones inconsistentes. Revísela desde Asistencia.");
                }


                return "ENTRADA";
            }


            if (!utilizaDescanso)
            {
                return "SALIDA";
            }


            if (!tieneInicioDescanso)
            {
                return "INICIO_DESCANSO";
            }


            if (!tieneFinDescanso)
            {
                return "FIN_DESCANSO";
            }


            return "SALIDA";
        }


        // =====================================================
        // CREAR MARCACIÓN POR CREDENCIAL
        // =====================================================

        private static MarcacionAsistencia
            CrearMarcacionCredencial(
                Empleado empleado,
                EmpleadoCredencial credencial,
                EmpleadoHorario? horario,
                DateTime fechaHora,
                string tipoMarcacion,
                string? origen)
        {
            return new MarcacionAsistencia
            {
                EmpleadoID =
                    empleado.ID,

                EmpleadoCredencialID =
                    credencial.ID,

                EmpleadoHorarioID =
                    horario?.ID,

                FechaHora =
                    fechaHora,

                TipoMarcacion =
                    tipoMarcacion,

                MetodoMarcacion =
                    credencial.TipoCredencial,

                Origen =
                    string.IsNullOrWhiteSpace(
                        origen)
                    ?
                    null
                    :
                    origen.Trim()
            };
        }


        // =====================================================
        // AGREGAR MARCACIÓN MANUAL
        // =====================================================

        private static void
            AgregarManualSiCorresponde(
                List<MarcacionAsistencia> nuevas,
                List<MarcacionAsistencia> existentes,
                Empleado empleado,
                EmpleadoHorario? horario,
                DateTime fecha,
                TimeOnly? hora,
                string tipoMarcacion,
                string? observacion)
        {
            if (!hora.HasValue)
                return;


            if (existentes.Any(x =>
                x.TipoMarcacion ==
                tipoMarcacion))
            {
                throw new InvalidOperationException(
                    $"Ya existe una marcación {FormatearTipo(tipoMarcacion)} para esa fecha.");
            }


            var fechaHora =
                fecha.Date.Add(
                    hora.Value.ToTimeSpan());


            if (fechaHora >
                DateTime.Now.AddMinutes(1))
            {
                throw new InvalidOperationException(
                    $"La hora de {FormatearTipo(tipoMarcacion)} no puede ser futura.");
            }


            nuevas.Add(
                new MarcacionAsistencia
                {
                    EmpleadoID =
                        empleado.ID,

                    EmpleadoCredencialID =
                        null,

                    EmpleadoHorarioID =
                        horario?.ID,

                    FechaHora =
                        fechaHora,

                    TipoMarcacion =
                        tipoMarcacion,

                    MetodoMarcacion =
                        "MANUAL",

                    Origen =
                        string.IsNullOrWhiteSpace(
                            observacion)
                        ?
                        "REGISTRO_MANUAL"
                        :
                        observacion.Trim()
                });
        }


        // =====================================================
        // VALIDAR JORNADA MANUAL
        // =====================================================

        private static void
            ValidarJornadaManual(
                List<MarcacionAsistencia> marcaciones)
        {
            var entrada =
                ObtenerFecha(
                    marcaciones,
                    "ENTRADA");


            var inicioDescanso =
                ObtenerFecha(
                    marcaciones,
                    "INICIO_DESCANSO");


            var finDescanso =
                ObtenerFecha(
                    marcaciones,
                    "FIN_DESCANSO");


            var salida =
                ObtenerFecha(
                    marcaciones,
                    "SALIDA");


            if (!entrada.HasValue
                &&
                (
                    inicioDescanso.HasValue
                    ||
                    finDescanso.HasValue
                    ||
                    salida.HasValue
                ))
            {
                throw new InvalidOperationException(
                    "Debe existir una entrada antes de las demás marcaciones.");
            }


            if (finDescanso.HasValue
                &&
                !inicioDescanso.HasValue)
            {
                throw new InvalidOperationException(
                    "No puede registrar regreso de descanso sin una salida a descanso.");
            }


            if (salida.HasValue
                &&
                inicioDescanso.HasValue
                &&
                !finDescanso.HasValue)
            {
                throw new InvalidOperationException(
                    "No puede registrar la salida final mientras el descanso está abierto.");
            }


            if (entrada.HasValue
                &&
                inicioDescanso.HasValue
                &&
                inicioDescanso <= entrada)
            {
                throw new InvalidOperationException(
                    "La salida a descanso debe ser posterior a la entrada.");
            }


            if (inicioDescanso.HasValue
                &&
                finDescanso.HasValue
                &&
                finDescanso <= inicioDescanso)
            {
                throw new InvalidOperationException(
                    "El regreso de descanso debe ser posterior a la salida a descanso.");
            }


            if (entrada.HasValue
                &&
                salida.HasValue
                &&
                salida <= entrada)
            {
                throw new InvalidOperationException(
                    "La salida debe ser posterior a la entrada.");
            }


            if (finDescanso.HasValue
                &&
                salida.HasValue
                &&
                salida <= finDescanso)
            {
                throw new InvalidOperationException(
                    "La salida final debe ser posterior al regreso del descanso.");
            }
        }


        // =====================================================
        // CONSTRUIR RESUMEN
        // =====================================================

        private static AsistenciaDiariaDto
            ConstruirResumenDiario(
                Empleado empleado,
                EmpleadoHorario? asignacion,
                HorarioDetalle? detalle,
                List<MarcacionAsistencia> marcaciones,
                DateTime fecha)
        {
            var entrada =
                ObtenerFecha(
                    marcaciones,
                    "ENTRADA");


            var inicioDescanso =
                ObtenerFecha(
                    marcaciones,
                    "INICIO_DESCANSO");


            var finDescanso =
                ObtenerFecha(
                    marcaciones,
                    "FIN_DESCANSO");


            var salida =
                ObtenerFecha(
                    marcaciones,
                    "SALIDA");


            bool esLaboral =
                detalle?.EsLaboral == true;


            int minutosProgramados =
                CalcularMinutosProgramados(
                    detalle);


            int minutosTrabajados =
                CalcularMinutosTrabajados(
                    fecha,
                    entrada,
                    inicioDescanso,
                    finDescanso,
                    salida);


            int minutosTarde =
                CalcularMinutosTarde(
                    fecha,
                    detalle,
                    entrada);


            int tolerancia =
                asignacion?.Horario?
                    .ToleranciaMinutos
                ??
                0;


            bool esTarde =
                minutosTarde >
                tolerancia;


            string estado =
                DeterminarEstadoAsistencia(
                    fecha,
                    detalle,
                    entrada,
                    inicioDescanso,
                    finDescanso,
                    salida,
                    tolerancia);


            bool jornadaCerrada =
                salida.HasValue;


            bool diaFinalizado =
                fecha.Date <
                DateTime.Today;


            int minutosFaltantes =
                jornadaCerrada
                ||
                diaFinalizado
                ?
                Math.Max(
                    0,
                    minutosProgramados
                    -
                    minutosTrabajados)
                :
                0;


            int minutosExtra =
                jornadaCerrada
                &&
                esLaboral
                ?
                Math.Max(
                    0,
                    minutosTrabajados
                    -
                    minutosProgramados)
                :
                0;


            string nombreHorario =
                asignacion?.Horario?.Nombre
                ??
                "Sin horario";


            if (detalle?.EsLaboral == true
                &&
                detalle.HoraEntrada.HasValue
                &&
                detalle.HoraSalida.HasValue)
            {
                nombreHorario +=
                    $" {detalle.HoraEntrada.Value:HH\\:mm}" +
                    $" - {detalle.HoraSalida.Value:HH\\:mm}";
            }


            return new AsistenciaDiariaDto
            {
                EmpleadoID =
                    empleado.ID,

                CodigoEmpleado =
                    empleado.Codigo,

                NombreEmpleado =
                    $"{empleado.Nombres} {empleado.Apellidos}"
                        .Trim(),

                Horario =
                    nombreHorario,

                EsLaboral =
                    esLaboral,

                HoraEntradaProgramada =
                    detalle?.HoraEntrada,

                HoraSalidaProgramada =
                    detalle?.HoraSalida,

                HoraEntrada =
                    entrada,

                HoraInicioDescanso =
                    inicioDescanso,

                HoraFinDescanso =
                    finDescanso,

                HoraSalida =
                    salida,

                MinutosProgramados =
                    minutosProgramados,

                MinutosTrabajados =
                    minutosTrabajados,

                MinutosTarde =
                    minutosTarde,

                MinutosFaltantes =
                    minutosFaltantes,

                MinutosExtraCalculados =
                    minutosExtra,

                ToleranciaMinutos =
                    tolerancia,

                EsTarde =
                    esTarde,

                EsPresente =
                    entrada.HasValue,

                SinSalida =
                    entrada.HasValue
                    &&
                    !salida.HasValue
                    &&
                    fecha.Date <
                    DateTime.Today,

                EstadoAsistencia =
                    estado
            };
        }


        // =====================================================
        // ESTADO DE ASISTENCIA
        // =====================================================

        private static string
            DeterminarEstadoAsistencia(
                DateTime fecha,
                HorarioDetalle? detalle,
                DateTime? entrada,
                DateTime? inicioDescanso,
                DateTime? finDescanso,
                DateTime? salida,
                int tolerancia)
        {
            var dia =
                fecha.Date;


            if (detalle == null)
            {
                if (entrada.HasValue)
                {
                    return salida.HasValue
                        ?
                        "Finalizado"
                        :
                        dia == DateTime.Today
                            ?
                            "Trabajando"
                            :
                            "Incompleto";
                }


                return "Sin horario";
            }


            if (!detalle.EsLaboral)
            {
                if (entrada.HasValue)
                {
                    return salida.HasValue
                        ?
                        "Finalizado"
                        :
                        dia == DateTime.Today
                            ?
                            "Trabajando"
                            :
                            "Incompleto";
                }


                return "No laboral";
            }


            if (!detalle.HoraEntrada.HasValue
                ||
                !detalle.HoraSalida.HasValue)
            {
                return "Horario incompleto";
            }


            if (!entrada.HasValue)
            {
                if (dia >
                    DateTime.Today)
                {
                    return "Pendiente";
                }


                if (dia ==
                    DateTime.Today)
                {
                    var limiteEntrada =
                        dia
                            .Add(
                                detalle.HoraEntrada.Value
                                    .ToTimeSpan())
                            .AddMinutes(
                                tolerancia);


                    if (DateTime.Now <=
                        limiteEntrada)
                    {
                        return "Pendiente";
                    }
                }


                return "Ausente";
            }


            if (inicioDescanso.HasValue
                &&
                !finDescanso.HasValue)
            {
                return dia ==
                    DateTime.Today
                    ?
                    "En descanso"
                    :
                    "Incompleto";
            }


            if (salida.HasValue)
            {
                return "Finalizado";
            }


            if (dia <
                DateTime.Today)
            {
                return "Incompleto";
            }


            return "Trabajando";
        }


        // =====================================================
        // MINUTOS PROGRAMADOS
        // =====================================================

        private static int
            CalcularMinutosProgramados(
                HorarioDetalle? detalle)
        {
            if (detalle == null
                ||
                !detalle.EsLaboral
                ||
                !detalle.HoraEntrada.HasValue
                ||
                !detalle.HoraSalida.HasValue)
            {
                return 0;
            }


            var total =
                detalle.HoraSalida.Value
                    .ToTimeSpan()
                -
                detalle.HoraEntrada.Value
                    .ToTimeSpan();


            int minutos =
                (int)total.TotalMinutes
                -
                detalle.MinutosDescanso;


            return Math.Max(
                0,
                minutos);
        }


        // =====================================================
        // MINUTOS TRABAJADOS
        // =====================================================

        private static int
            CalcularMinutosTrabajados(
                DateTime fecha,
                DateTime? entrada,
                DateTime? inicioDescanso,
                DateTime? finDescanso,
                DateTime? salida)
        {
            if (!entrada.HasValue)
                return 0;


            // Jornada cerrada.
            if (salida.HasValue)
            {
                double minutos =
                    (
                        salida.Value
                        -
                        entrada.Value
                    ).TotalMinutes;


                if (inicioDescanso.HasValue
                    &&
                    finDescanso.HasValue)
                {
                    minutos -=
                        (
                            finDescanso.Value
                            -
                            inicioDescanso.Value
                        ).TotalMinutes;
                }
                else if (inicioDescanso.HasValue
                    &&
                    !finDescanso.HasValue)
                {
                    // Descanso sin cerrar.
                    minutos =
                        (
                            inicioDescanso.Value
                            -
                            entrada.Value
                        ).TotalMinutes;
                }


                return Math.Max(
                    0,
                    (int)Math.Floor(minutos));
            }


            // Jornada actual todavía abierta.
            if (fecha.Date ==
                DateTime.Today)
            {
                if (inicioDescanso.HasValue
                    &&
                    !finDescanso.HasValue)
                {
                    return Math.Max(
                        0,
                        (int)Math.Floor(
                            (
                                inicioDescanso.Value
                                -
                                entrada.Value
                            ).TotalMinutes));
                }


                double minutos =
                    (
                        DateTime.Now
                        -
                        entrada.Value
                    ).TotalMinutes;


                if (inicioDescanso.HasValue
                    &&
                    finDescanso.HasValue)
                {
                    minutos -=
                        (
                            finDescanso.Value
                            -
                            inicioDescanso.Value
                        ).TotalMinutes;
                }


                return Math.Max(
                    0,
                    (int)Math.Floor(minutos));
            }


            // Día anterior sin salida.
            // Solo contamos intervalos que realmente conocemos.
            if (inicioDescanso.HasValue)
            {
                return Math.Max(
                    0,
                    (int)Math.Floor(
                        (
                            inicioDescanso.Value
                            -
                            entrada.Value
                        ).TotalMinutes));
            }


            return 0;
        }


        // =====================================================
        // TARDANZA
        // =====================================================

        private static int
            CalcularMinutosTarde(
                DateTime fecha,
                HorarioDetalle? detalle,
                DateTime? entrada)
        {
            if (detalle == null
                ||
                !detalle.EsLaboral
                ||
                !detalle.HoraEntrada.HasValue
                ||
                !entrada.HasValue)
            {
                return 0;
            }


            var programada =
                fecha.Date.Add(
                    detalle.HoraEntrada.Value
                        .ToTimeSpan());


            var entradaMinuto =
                TruncarMinuto(
                    entrada.Value);


            var programadaMinuto =
                TruncarMinuto(
                    programada);


            if (entradaMinuto <=
                programadaMinuto)
            {
                return 0;
            }


            return Math.Max(
                0,
                (int)(
                    entradaMinuto
                    -
                    programadaMinuto
                ).TotalMinutes);
        }


        // =====================================================
        // AUXILIARES
        // =====================================================

        private static DateTime?
            ObtenerFecha(
                IEnumerable<MarcacionAsistencia> marcaciones,
                string tipo)
        {
            return marcaciones
                .Where(x =>
                    string.Equals(
                        x.TipoMarcacion,
                        tipo,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(x =>
                    x.FechaHora)
                .Select(x =>
                    (DateTime?)x.FechaHora)
                .FirstOrDefault();
        }


        private static int
            ConvertirDiaSemana(
                DateTime fecha)
        {
            int dia =
                (int)fecha.DayOfWeek;


            return dia == 0
                ?
                7
                :
                dia;
        }


        private static DateTime
            TruncarMinuto(
                DateTime valor)
        {
            return new DateTime(
                valor.Year,
                valor.Month,
                valor.Day,
                valor.Hour,
                valor.Minute,
                0,
                valor.Kind);
        }


        private static string
            NormalizarTipoMarcacion(
                string tipo)
        {
            if (string.IsNullOrWhiteSpace(
                tipo))
            {
                throw new InvalidOperationException(
                    "Debe indicar el tipo de marcación.");
            }


            var normalizado =
                tipo.Trim()
                    .ToUpperInvariant();


            if (!TiposPermitidos.Contains(
                normalizado))
            {
                throw new InvalidOperationException(
                    "Tipo de marcación no válido.");
            }


            return normalizado;
        }


        private static string
            FormatearTipo(
                string tipo)
        {
            return tipo switch
            {
                "ENTRADA" =>
                    "Entrada",

                "INICIO_DESCANSO" =>
                    "Salida a descanso",

                "FIN_DESCANSO" =>
                    "Regreso de descanso",

                "SALIDA" =>
                    "Salida",

                _ =>
                    tipo
            };
        }


        private static MarcacionAsistenciaDto
            MapearDto(
                MarcacionAsistencia marcacion,
                Empleado empleado)
        {
            return new MarcacionAsistenciaDto
            {
                ID =
                    marcacion.ID,

                EmpleadoID =
                    marcacion.EmpleadoID,

                EmpleadoCredencialID =
                    marcacion.EmpleadoCredencialID,

                EmpleadoHorarioID =
                    marcacion.EmpleadoHorarioID,

                CodigoEmpleado =
                    empleado.Codigo,

                NombreEmpleado =
                    $"{empleado.Nombres} {empleado.Apellidos}"
                        .Trim(),

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