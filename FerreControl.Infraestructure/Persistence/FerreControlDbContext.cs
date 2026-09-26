using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public DbSet<Negocio> Negocios => Set<Negocio>();

    public DbSet<Empleado> Empleados => Set<Empleado>();

    public DbSet<SalarioEmpleado> SalariosEmpleado =>
        Set<SalarioEmpleado>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


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
    }
}