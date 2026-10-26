using FerreControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FerreControl.Infrastructure.Persistence;

public class FerreControlDbContext : DbContext
{
    public FerreControlDbContext(
        DbContextOptions<FerreControlDbContext> options)
        : base(options)
    {
    }

    // =========================================================
    // DBSETS
    // =========================================================

    public DbSet<Negocio> Negocios => Set<Negocio>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<SalarioEmpleado> SalariosEmpleado => Set<SalarioEmpleado>();
    public DbSet<Horario> Horarios => Set<Horario>();
    public DbSet<HorarioDetalle> HorariosDetalle => Set<HorarioDetalle>();
    public DbSet<EmpleadoHorario> EmpleadosHorarios => Set<EmpleadoHorario>();
    public DbSet<EmpleadoCredencial> EmpleadosCredenciales => Set<EmpleadoCredencial>();
    public DbSet<MarcacionAsistencia> MarcacionesAsistencia => Set<MarcacionAsistencia>();
    public DbSet<Nomina> Nominas => Set<Nomina>();
    public DbSet<NominaDetalle> NominasDetalle => Set<NominaDetalle>();
    public DbSet<PagoNomina> PagosNomina => Set<PagoNomina>();
    public DbSet<AdelantoEmpleado> AdelantosEmpleado => Set<AdelantoEmpleado>();
    public DbSet<AdelantoAplicacion> AdelantosAplicaciones => Set<AdelantoAplicacion>();

    public DbSet<ReglaHoraExtra> ReglasHoraExtra => Set<ReglaHoraExtra>();
    public DbSet<EmpleadoReglaHoraExtra> EmpleadosReglasHoraExtra => Set<EmpleadoReglaHoraExtra>();
    public DbSet<RegistroHoraExtra> RegistrosHoraExtra => Set<RegistroHoraExtra>();

    // =========================================================
    // CONFIGURACION DEL MODELO
    // =========================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // NEGOCIO
        // =====================================================

        modelBuilder.Entity<Negocio>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Tipo)
                .HasMaxLength(50)
                .IsRequired();
        });

        // =====================================================
        // EMPLEADO
        // =====================================================

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Codigo)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Nombres)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Apellidos)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Telefono)
                .HasMaxLength(30);

            entity.Property(x => x.Puesto)
                .HasMaxLength(100);

            entity.HasIndex(x => new
            {
                x.NegocioID,
                x.Codigo
            })
            .IsUnique();

            entity.HasOne(x => x.Negocio)
                .WithMany(x => x.Empleados)
                .HasForeignKey(x => x.NegocioID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // SALARIO EMPLEADO
        // =====================================================

        modelBuilder.Entity<SalarioEmpleado>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Periodicidad)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.Salarios)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // HORARIO
        // =====================================================

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(x => x.Negocio)
                .WithMany(x => x.Horarios)
                .HasForeignKey(x => x.NegocioID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // HORARIO DETALLE
        // =====================================================

        modelBuilder.Entity<HorarioDetalle>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.HasIndex(x => new
            {
                x.HorarioID,
                x.DiaSemana
            })
            .IsUnique();

            entity.HasOne(x => x.Horario)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.HorarioID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // EMPLEADO HORARIO
        // =====================================================

        modelBuilder.Entity<EmpleadoHorario>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.HasIndex(x => new
            {
                x.EmpleadoID,
                x.FechaInicio
            });

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.Horarios)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Horario)
                .WithMany(x => x.EmpleadosHorarios)
                .HasForeignKey(x => x.HorarioID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // EMPLEADO CREDENCIAL
        // =====================================================

        modelBuilder.Entity<EmpleadoCredencial>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.TipoCredencial)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Codigo)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.TipoCredencial,
                x.Codigo
            })
            .IsUnique();

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.Credenciales)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // MARCACION ASISTENCIA
        // =====================================================

        modelBuilder.Entity<MarcacionAsistencia>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.TipoMarcacion)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.MetodoMarcacion)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Origen)
                .HasMaxLength(100);

            entity.HasIndex(x => new
            {
                x.EmpleadoID,
                x.FechaHora
            });

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.Marcaciones)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EmpleadoCredencial)
                .WithMany(x => x.Marcaciones)
                .HasForeignKey(x => x.EmpleadoCredencialID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EmpleadoHorario)
                .WithMany(x => x.Marcaciones)
                .HasForeignKey(x => x.EmpleadoHorarioID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // NOMINA DETALLE
        // =====================================================

        modelBuilder.Entity<NominaDetalle>()
            .HasIndex(x => new
            {
                x.NominaID,
                x.EmpleadoID
            })
            .IsUnique();

        // =====================================================
        // ADELANTO EMPLEADO
        // =====================================================

        modelBuilder.Entity<AdelantoEmpleado>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.MetodoEntrega)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Referencia)
                .HasMaxLength(100);

            entity.Property(x => x.Observacion)
                .HasMaxLength(500);

            entity.Property(x => x.Estado)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.MotivoAnulacion)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.EmpleadoID,
                x.FechaAdelanto
            });

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.Adelantos)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // ADELANTO APLICACION
        // =====================================================

        modelBuilder.Entity<AdelantoAplicacion>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Observacion)
                .HasMaxLength(500);

            entity.Property(x => x.MotivoAnulacion)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.AdelantoEmpleadoID,
                x.NominaDetalleID
            });

            entity.HasOne(x => x.AdelantoEmpleado)
                .WithMany(x => x.Aplicaciones)
                .HasForeignKey(x => x.AdelantoEmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.NominaDetalle)
                .WithMany(x => x.AdelantosAplicados)
                .HasForeignKey(x => x.NominaDetalleID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // REGLA HORA EXTRA
        // =====================================================

        modelBuilder.Entity<ReglaHoraExtra>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Nombre)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.TipoFraccion)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.NegocioID,
                x.Nombre
            })
            .IsUnique();

            entity.HasOne(x => x.Negocio)
                .WithMany(x => x.ReglasHoraExtra)
                .HasForeignKey(x => x.NegocioID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // EMPLEADO - REGLA HORA EXTRA
        // =====================================================

        modelBuilder.Entity<EmpleadoReglaHoraExtra>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.HasIndex(x => new
            {
                x.EmpleadoID,
                x.FechaInicio
            });

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.ConfiguracionesHoraExtra)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ReglaHoraExtra)
                .WithMany(x => x.Asignaciones)
                .HasForeignKey(x => x.ReglaHoraExtraID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // REGISTRO HORA EXTRA
        // =====================================================

        modelBuilder.Entity<RegistroHoraExtra>(entity =>
        {
            entity.HasKey(x => x.ID);

            entity.Property(x => x.Estado)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Observacion)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.EmpleadoID,
                x.Fecha
            })
            .IsUnique();

            entity.HasOne(x => x.Empleado)
                .WithMany(x => x.RegistrosHoraExtra)
                .HasForeignKey(x => x.EmpleadoID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EmpleadoReglaHoraExtra)
                .WithMany(x => x.Registros)
                .HasForeignKey(x => x.EmpleadoReglaHoraExtraID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
