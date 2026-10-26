using FerreControl.Application.DTOs.HorasExtra;
using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Domain.Entities;

namespace FerreControl.Infraestructure.Services;

public class HoraExtraService
    : IHoraExtraService
{
    private readonly IReglaHoraExtraRepository _reglaRepository;
    private readonly IEmpleadoReglaHoraExtraRepository _configuracionRepository;
    private readonly IRegistroHoraExtraRepository _registroRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IEmpleadoHorarioRepository _empleadoHorarioRepository;
    private readonly IHorarioDetalleRepository _horarioDetalleRepository;
    private readonly IMarcacionAsistenciaRepository _marcacionRepository;
    private readonly INominaRepository _nominaRepository;
    private readonly INominaDetalleRepository _nominaDetalleRepository;

    public HoraExtraService(
        IReglaHoraExtraRepository reglaRepository,
        IEmpleadoReglaHoraExtraRepository configuracionRepository,
        IRegistroHoraExtraRepository registroRepository,
        IEmpleadoRepository empleadoRepository,
        IEmpleadoHorarioRepository empleadoHorarioRepository,
        IHorarioDetalleRepository horarioDetalleRepository,
        IMarcacionAsistenciaRepository marcacionRepository,
        INominaRepository nominaRepository,
        INominaDetalleRepository nominaDetalleRepository)
    {
        _reglaRepository = reglaRepository;
        _configuracionRepository = configuracionRepository;
        _registroRepository = registroRepository;
        _empleadoRepository = empleadoRepository;
        _empleadoHorarioRepository = empleadoHorarioRepository;
        _horarioDetalleRepository = horarioDetalleRepository;
        _marcacionRepository = marcacionRepository;
        _nominaRepository = nominaRepository;
        _nominaDetalleRepository = nominaDetalleRepository;
    }

    // =========================================================
    // REGLAS
    // =========================================================

    public async Task<List<ReglaHoraExtraDto>>
        ObtenerReglasAsync(int negocioId)
    {
        var reglas =
            await _reglaRepository
                .ObtenerPorNegocioAsync(negocioId);

        return reglas
            .Select(MapearRegla)
            .ToList();
    }

    public async Task<ReglaHoraExtraDto>
        CrearReglaAsync(
            CrearReglaHoraExtraDto dto)
    {
        if (dto.NegocioID <= 0)
            throw new InvalidOperationException("Debe indicar el negocio.");

        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new InvalidOperationException("El nombre de la regla es obligatorio.");

        if (dto.TarifaHora <= 0)
            throw new InvalidOperationException("La tarifa por hora extra debe ser mayor a cero.");

        var reglas =
            await _reglaRepository
                .ObtenerPorNegocioAsync(dto.NegocioID);

        if (reglas.Any(x =>
            string.Equals(
                x.Nombre,
                dto.Nombre.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Ya existe una regla de horas extra con ese nombre.");
        }

        var regla =
            new ReglaHoraExtra
            {
                NegocioID = dto.NegocioID,
                Nombre = dto.Nombre.Trim(),
                TarifaHoraCentavos = A_Centavos(dto.TarifaHora),
                TipoFraccion = "EXACTO",
                RequiereAprobacion = dto.RequiereAprobacion,
                CuentaEntradaAnticipada = dto.CuentaEntradaAnticipada,
                Estado = true,
                FechaCreacion = DateTime.Now
            };

        await _reglaRepository.AgregarAsync(regla);

        return MapearRegla(regla);
    }

    // =========================================================
    // ASIGNACIÓN
    // =========================================================

    public async Task<EmpleadoReglaHoraExtraDto?>
        ObtenerConfiguracionActualAsync(
            int empleadoId)
    {
        var configuracion =
            await _configuracionRepository
                .ObtenerActualAsync(empleadoId);

        return configuracion == null
            ? null
            : MapearConfiguracion(configuracion);
    }

    public async Task<EmpleadoReglaHoraExtraDto>
        AsignarReglaAsync(
            int empleadoId,
            AsignarReglaHoraExtraDto dto)
    {
        var empleado =
            await _empleadoRepository
                .ObtenerPorIdAsync(empleadoId);

        if (empleado == null)
            throw new InvalidOperationException("El empleado no existe.");

        if (!empleado.Estado)
            throw new InvalidOperationException(
                "No puede asignar una regla de horas extra a un empleado inactivo.");

        var regla =
            await _reglaRepository
                .ObtenerPorIdAsync(dto.ReglaHoraExtraID);

        if (regla == null)
            throw new InvalidOperationException("La regla de horas extra no existe.");

        if (!regla.Estado)
            throw new InvalidOperationException("La regla de horas extra se encuentra inactiva.");

        if (regla.NegocioID != empleado.NegocioID)
            throw new InvalidOperationException(
                "La regla no pertenece al mismo negocio del empleado.");

        var fechaInicio = dto.FechaInicio.Date;

        if (fechaInicio > DateTime.Today)
            throw new InvalidOperationException(
                "Por ahora la fecha de inicio no puede ser futura.");

        if (empleado.FechaIngreso.HasValue &&
            fechaInicio < empleado.FechaIngreso.Value.Date)
        {
            throw new InvalidOperationException(
                "La regla no puede iniciar antes de la fecha de ingreso del empleado.");
        }

        var actual =
            await _configuracionRepository
                .ObtenerActualAsync(empleadoId);

        if (actual != null)
        {
            if (actual.ReglaHoraExtraID == regla.ID)
                throw new InvalidOperationException(
                    "El empleado ya tiene esta regla de horas extra asignada.");

            if (fechaInicio <= actual.FechaInicio.Date)
                throw new InvalidOperationException(
                    "La nueva regla debe iniciar después de la asignación actual.");

            actual.FechaFin = fechaInicio.AddDays(-1);
            actual.Estado = false;

            await _configuracionRepository
                .ActualizarAsync(actual);
        }

        var nueva =
            new EmpleadoReglaHoraExtra
            {
                EmpleadoID = empleadoId,
                ReglaHoraExtraID = regla.ID,
                FechaInicio = fechaInicio,
                Estado = true,
                FechaCreacion = DateTime.Now
            };

        await _configuracionRepository
            .AgregarAsync(nueva);

        nueva.ReglaHoraExtra = regla;

        return MapearConfiguracion(nueva);
    }

    // =========================================================
    // DESACTIVAR CONFIGURACIÓN DEL EMPLEADO
    // =========================================================

    public async Task<EmpleadoReglaHoraExtraDto>
        DesactivarConfiguracionAsync(
            int empleadoId,
            DesactivarHoraExtraEmpleadoDto dto)
    {
        var empleado =
            await _empleadoRepository
                .ObtenerPorIdAsync(empleadoId);

        if (empleado == null)
            throw new InvalidOperationException(
                "El empleado no existe.");

        var actual =
            await _configuracionRepository
                .ObtenerActualAsync(empleadoId);

        if (actual == null)
            throw new InvalidOperationException(
                "El empleado no tiene una configuración de horas extra activa.");

        var fechaInicioNoAplica =
            dto.FechaInicioNoAplica.Date;

        if (fechaInicioNoAplica <= actual.FechaInicio.Date)
        {
            throw new InvalidOperationException(
                "La fecha en que deja de aplicar horas extra debe ser posterior " +
                "al inicio de la configuración actual.");
        }

        actual.FechaFin =
            fechaInicioNoAplica.AddDays(-1);

        actual.Estado = false;

        await _configuracionRepository
            .ActualizarAsync(actual);

        return MapearConfiguracion(actual);
    }

    // =========================================================
    // ASISTENCIA -> EXTRA CANDIDATO
    // =========================================================

    public async Task<List<RegistroHoraExtraDto>>
        CalcularPeriodoAsync(
            CalcularHorasExtraDto dto)
    {
        ValidarPeriodo(dto.Desde, dto.Hasta);

        var desde = dto.Desde.Date;
        var hasta = dto.Hasta.Date;

        var empleados =
            (await _empleadoRepository.ObtenerTodosAsync())
            .Where(x =>
                x.NegocioID == dto.NegocioID &&
                (!x.FechaIngreso.HasValue || x.FechaIngreso.Value.Date <= hasta) &&
                (!x.FechaBaja.HasValue || x.FechaBaja.Value.Date >= desde))
            .ToList();

        foreach (var empleado in empleados)
        {
            for (var fecha = desde;
                 fecha <= hasta;
                 fecha = fecha.AddDays(1))
            {
                await CalcularDiaAsync(
                    empleado,
                    fecha);
            }
        }

        return await ObtenerRegistrosAsync(
            dto.NegocioID,
            desde,
            hasta);
    }

    private async Task CalcularDiaAsync(
        Empleado empleado,
        DateTime fecha)
    {
        var configuracion =
            await _configuracionRepository
                .ObtenerVigenteEnFechaAsync(
                    empleado.ID,
                    fecha);

        if (configuracion == null ||
            !configuracion.ReglaHoraExtra.Estado)
        {
            return;
        }

        var empleadoHorario =
            await _empleadoHorarioRepository
                .ObtenerVigenteEnFechaAsync(
                    empleado.ID,
                    fecha);

        if (empleadoHorario == null)
            return;

        var detalles =
            await _horarioDetalleRepository
                .ObtenerPorHorarioAsync(
                    empleadoHorario.HorarioID);

        int diaSemana =
            ConvertirDiaSemana(fecha);

        var detalle =
            detalles.FirstOrDefault(x =>
                x.DiaSemana == diaSemana);

        // Primera versión:
        // no automatizamos trabajo en día de descanso/feriado.
        if (detalle == null ||
            !detalle.EsLaboral ||
            !detalle.HoraEntrada.HasValue ||
            !detalle.HoraSalida.HasValue)
        {
            return;
        }

        var marcaciones =
            await _marcacionRepository
                .ObtenerPorEmpleadoYFechaAsync(
                    empleado.ID,
                    fecha);

        var entrada =
            ObtenerMarcacion(
                marcaciones,
                "ENTRADA");

        var salida =
            ObtenerMarcacion(
                marcaciones,
                "SALIDA");

        if (!entrada.HasValue ||
            !salida.HasValue ||
            salida <= entrada)
        {
            return;
        }

        var inicioProgramado =
            fecha.Date.Add(
                detalle.HoraEntrada.Value.ToTimeSpan());

        var finProgramado =
            fecha.Date.Add(
                detalle.HoraSalida.Value.ToTimeSpan());

        int minutosExtra = 0;

        if (configuracion.ReglaHoraExtra.CuentaEntradaAnticipada &&
            entrada.Value < inicioProgramado)
        {
            minutosExtra +=
                MinutosCompletos(
                    inicioProgramado - entrada.Value);
        }

        if (salida.Value > finProgramado)
        {
            minutosExtra +=
                MinutosCompletos(
                    salida.Value - finProgramado);
        }

        int minutosTrabajados =
            CalcularMinutosTrabajados(
                marcaciones,
                entrada.Value,
                salida.Value);

        bool excede12Horas =
            minutosTrabajados > 12 * 60;

        var existente =
            await _registroRepository
                .ObtenerPorEmpleadoFechaAsync(
                    empleado.ID,
                    fecha);

        // Una decisión ya aprobada o rechazada queda congelada.
        if (existente != null &&
            (
                existente.Estado == EstadosHoraExtra.APROBADO ||
                existente.Estado == EstadosHoraExtra.RECHAZADO
            ))
        {
            return;
        }

        long tarifa =
            configuracion.ReglaHoraExtra.TarifaHoraCentavos;

        string estado =
            minutosExtra <= 0
                ? EstadosHoraExtra.SIN_EXTRA
                : configuracion.ReglaHoraExtra.RequiereAprobacion
                    ? EstadosHoraExtra.PENDIENTE
                    : EstadosHoraExtra.APROBADO;

        int minutosAprobados =
            estado == EstadosHoraExtra.APROBADO
                ? minutosExtra
                : 0;

        long monto =
            CalcularMontoCentavos(
                tarifa,
                minutosAprobados);

        if (existente == null)
        {
            existente =
                new RegistroHoraExtra
                {
                    EmpleadoID = empleado.ID,
                    EmpleadoReglaHoraExtraID = configuracion.ID,
                    Fecha = fecha.Date,
                    MinutosCalculados = minutosExtra,
                    MinutosAprobados = minutosAprobados,
                    TarifaHoraCentavos = tarifa,
                    MontoCentavos = monto,
                    Estado = estado,
                    ExcedeLimite12Horas = excede12Horas,
                    FechaCalculo = DateTime.Now,
                    FechaDecision =
                        estado == EstadosHoraExtra.APROBADO
                            ? DateTime.Now
                            : null
                };

            await _registroRepository
                .AgregarAsync(existente);
        }
        else
        {
            existente.EmpleadoReglaHoraExtraID = configuracion.ID;
            existente.MinutosCalculados = minutosExtra;
            existente.MinutosAprobados = minutosAprobados;
            existente.TarifaHoraCentavos = tarifa;
            existente.MontoCentavos = monto;
            existente.Estado = estado;
            existente.ExcedeLimite12Horas = excede12Horas;
            existente.FechaCalculo = DateTime.Now;
            existente.FechaDecision =
                estado == EstadosHoraExtra.APROBADO
                    ? DateTime.Now
                    : null;

            await _registroRepository
                .ActualizarAsync(existente);
        }
    }

    // =========================================================
    // CONSULTA / APROBACIÓN
    // =========================================================

    public async Task<List<RegistroHoraExtraDto>>
        ObtenerRegistrosAsync(
            int negocioId,
            DateTime desde,
            DateTime hasta)
    {
        ValidarPeriodo(desde, hasta);

        var registros =
            await _registroRepository
                .ObtenerPorNegocioPeriodoAsync(
                    negocioId,
                    desde,
                    hasta);

        return registros
            .Select(MapearRegistro)
            .ToList();
    }

    public async Task<RegistroHoraExtraDto>
        AprobarAsync(
            int registroId,
            AprobarHoraExtraDto dto)
    {
        var registro =
            await _registroRepository
                .ObtenerPorIdAsync(registroId);

        if (registro == null)
            throw new InvalidOperationException(
                "El registro de horas extra no existe.");

        if (dto.MinutosAprobados < 0)
            throw new InvalidOperationException(
                "Los minutos aprobados no pueden ser negativos.");

        if (dto.MinutosAprobados > registro.MinutosCalculados)
            throw new InvalidOperationException(
                "No puede aprobar más minutos que los detectados por Asistencia.");

        if (registro.MinutosCalculados <= 0)
            throw new InvalidOperationException(
                "Este registro no tiene minutos extra para aprobar.");

        registro.MinutosAprobados = dto.MinutosAprobados;
        registro.MontoCentavos =
            CalcularMontoCentavos(
                registro.TarifaHoraCentavos,
                dto.MinutosAprobados);

        registro.Estado =
            dto.MinutosAprobados > 0
                ? EstadosHoraExtra.APROBADO
                : EstadosHoraExtra.RECHAZADO;

        registro.Observacion =
            string.IsNullOrWhiteSpace(dto.Observacion)
                ? null
                : dto.Observacion.Trim();

        registro.FechaDecision = DateTime.Now;

        await _registroRepository
            .ActualizarAsync(registro);

        return MapearRegistro(registro);
    }

    public async Task<RegistroHoraExtraDto>
        RechazarAsync(
            int registroId,
            RechazarHoraExtraDto dto)
    {
        var registro =
            await _registroRepository
                .ObtenerPorIdAsync(registroId);

        if (registro == null)
            throw new InvalidOperationException(
                "El registro de horas extra no existe.");

        registro.MinutosAprobados = 0;
        registro.MontoCentavos = 0;
        registro.Estado = EstadosHoraExtra.RECHAZADO;
        registro.Observacion =
            string.IsNullOrWhiteSpace(dto.Observacion)
                ? null
                : dto.Observacion.Trim();

        registro.FechaDecision = DateTime.Now;

        await _registroRepository
            .ActualizarAsync(registro);

        return MapearRegistro(registro);
    }

    // =========================================================
    // NÓMINA
    // =========================================================

    public async Task<ResultadoRecalculoNominaHoraExtraDto>
        RecalcularNominaAsync(
            int nominaId)
    {
        var nomina =
            await _nominaRepository
                .ObtenerPorIdAsync(nominaId);

        if (nomina == null)
            throw new InvalidOperationException(
                "La nómina no existe.");

        if (!string.Equals(
            nomina.Estado,
            "BORRADOR",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Las horas extra solo pueden recalcularse mientras la nómina está en BORRADOR.");
        }

        await CalcularPeriodoAsync(
            new CalcularHorasExtraDto
            {
                NegocioID = nomina.NegocioID,
                Desde = nomina.FechaInicioPeriodo,
                Hasta = nomina.FechaFinPeriodo
            });

        var registros =
            await _registroRepository
                .ObtenerPorNegocioPeriodoAsync(
                    nomina.NegocioID,
                    nomina.FechaInicioPeriodo,
                    nomina.FechaFinPeriodo);

        var aprobados =
            registros
                .Where(x =>
                    x.Estado == EstadosHoraExtra.APROBADO)
                .ToList();

        var detalles =
            await _nominaDetalleRepository
                .ObtenerPorNominaAsync(nominaId);

        int totalMinutos = 0;
        long totalMonto = 0;

        foreach (var detalle in detalles)
        {
            var delEmpleado =
                aprobados
                    .Where(x =>
                        x.EmpleadoID == detalle.EmpleadoID)
                    .ToList();

            int minutos =
                delEmpleado.Sum(x =>
                    x.MinutosAprobados);

            long monto =
                delEmpleado.Sum(x =>
                    x.MontoCentavos);

            detalle.MinutosExtra = minutos;
            detalle.HorasExtraCentavos = monto;

            long salarioParaTotal =
                detalle.SalarioDevengadoCentavos > 0
                    ? detalle.SalarioDevengadoCentavos
                    : detalle.SalarioBaseCentavos;

            detalle.TotalPagarCentavos =
                salarioParaTotal
                + detalle.BonificacionesCentavos
                + detalle.HorasExtraCentavos
                - detalle.AdelantosCentavos
                - detalle.OtrosDescuentosCentavos;

            if (detalle.TotalPagarCentavos < 0)
                detalle.TotalPagarCentavos = 0;

            await _nominaDetalleRepository
                .ActualizarAsync(detalle);

            totalMinutos += minutos;
            totalMonto += monto;
        }

        return new ResultadoRecalculoNominaHoraExtraDto
        {
            NominaID = nominaId,
            DetallesActualizados = detalles.Count,
            MinutosExtraAprobados = totalMinutos,
            MontoHorasExtra = A_Decimal(totalMonto)
        };
    }

    // =========================================================
    // CÁLCULOS
    // =========================================================

    private static int CalcularMinutosTrabajados(
        List<MarcacionAsistencia> marcaciones,
        DateTime entrada,
        DateTime salida)
    {
        double minutos =
            (salida - entrada).TotalMinutes;

        var inicioDescanso =
            ObtenerMarcacion(
                marcaciones,
                "INICIO_DESCANSO");

        var finDescanso =
            ObtenerMarcacion(
                marcaciones,
                "FIN_DESCANSO");

        if (inicioDescanso.HasValue &&
            finDescanso.HasValue &&
            finDescanso > inicioDescanso)
        {
            minutos -=
                (
                    finDescanso.Value
                    - inicioDescanso.Value
                ).TotalMinutes;
        }

        return Math.Max(
            0,
            (int)Math.Floor(minutos));
    }

    private static DateTime? ObtenerMarcacion(
        IEnumerable<MarcacionAsistencia> marcaciones,
        string tipo)
    {
        return marcaciones
            .Where(x =>
                string.Equals(
                    x.TipoMarcacion,
                    tipo,
                    StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.FechaHora)
            .Select(x => (DateTime?)x.FechaHora)
            .FirstOrDefault();
    }

    private static int MinutosCompletos(
        TimeSpan tiempo)
    {
        return Math.Max(
            0,
            (int)Math.Floor(
                tiempo.TotalMinutes));
    }

    private static int ConvertirDiaSemana(
        DateTime fecha)
    {
        int valor =
            (int)fecha.DayOfWeek;

        return valor == 0
            ? 7
            : valor;
    }

    private static long CalcularMontoCentavos(
        long tarifaHoraCentavos,
        int minutos)
    {
        if (tarifaHoraCentavos <= 0 ||
            minutos <= 0)
        {
            return 0;
        }

        decimal monto =
            tarifaHoraCentavos
            * (minutos / 60m);

        return (long)Math.Round(
            monto,
            MidpointRounding.AwayFromZero);
    }

    private static void ValidarPeriodo(
        DateTime desde,
        DateTime hasta)
    {
        if (desde.Date > hasta.Date)
            throw new InvalidOperationException(
                "La fecha inicial no puede ser posterior a la fecha final.");

        if ((hasta.Date - desde.Date).TotalDays > 62)
            throw new InvalidOperationException(
                "El período máximo para calcular horas extra es de 63 días.");
    }

    // =========================================================
    // MAPEOS
    // =========================================================

    private static ReglaHoraExtraDto MapearRegla(
        ReglaHoraExtra regla)
    {
        return new ReglaHoraExtraDto
        {
            ID = regla.ID,
            NegocioID = regla.NegocioID,
            Nombre = regla.Nombre,
            TarifaHora = A_Decimal(regla.TarifaHoraCentavos),
            TipoFraccion = regla.TipoFraccion,
            RequiereAprobacion = regla.RequiereAprobacion,
            CuentaEntradaAnticipada = regla.CuentaEntradaAnticipada,
            Estado = regla.Estado,
            FechaCreacion = regla.FechaCreacion
        };
    }

    private static EmpleadoReglaHoraExtraDto
        MapearConfiguracion(
            EmpleadoReglaHoraExtra configuracion)
    {
        return new EmpleadoReglaHoraExtraDto
        {
            ID = configuracion.ID,
            EmpleadoID = configuracion.EmpleadoID,
            ReglaHoraExtraID = configuracion.ReglaHoraExtraID,
            ReglaNombre =
                configuracion.ReglaHoraExtra?.Nombre
                ?? string.Empty,
            TarifaHora =
                configuracion.ReglaHoraExtra == null
                    ? 0
                    : A_Decimal(
                        configuracion
                            .ReglaHoraExtra
                            .TarifaHoraCentavos),
            FechaInicio = configuracion.FechaInicio,
            FechaFin = configuracion.FechaFin,
            Estado = configuracion.Estado
        };
    }

    private static RegistroHoraExtraDto MapearRegistro(
        RegistroHoraExtra registro)
    {
        var reglaNombre =
            registro.EmpleadoReglaHoraExtra?
                .ReglaHoraExtra?
                .Nombre
            ?? string.Empty;

        return new RegistroHoraExtraDto
        {
            ID = registro.ID,
            EmpleadoID = registro.EmpleadoID,
            CodigoEmpleado =
                registro.Empleado?.Codigo
                ?? string.Empty,
            NombreEmpleado =
                registro.Empleado == null
                    ? $"Empleado {registro.EmpleadoID}"
                    : $"{registro.Empleado.Nombres} {registro.Empleado.Apellidos}".Trim(),
            Fecha = registro.Fecha,
            ReglaNombre = reglaNombre,
            MinutosCalculados = registro.MinutosCalculados,
            MinutosAprobados = registro.MinutosAprobados,
            TarifaHora = A_Decimal(registro.TarifaHoraCentavos),
            MontoEstimado =
                A_Decimal(
                    CalcularMontoCentavos(
                        registro.TarifaHoraCentavos,
                        registro.MinutosCalculados)),
            MontoAprobado = A_Decimal(registro.MontoCentavos),
            Estado = registro.Estado,
            ExcedeLimite12Horas = registro.ExcedeLimite12Horas,
            Observacion = registro.Observacion,
            FechaCalculo = registro.FechaCalculo,
            FechaDecision = registro.FechaDecision
        };
    }

    private static long A_Centavos(
        decimal monto)
    {
        return (long)Math.Round(
            monto * 100m,
            MidpointRounding.AwayFromZero);
    }

    private static decimal A_Decimal(
        long centavos)
    {
        return centavos / 100m;
    }
}
