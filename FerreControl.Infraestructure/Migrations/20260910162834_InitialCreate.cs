using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Negocios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negocios", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NegocioID = table.Column<int>(type: "INTEGER", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Nombres = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: true),
                    Puesto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Empleados_Negocios_NegocioID",
                        column: x => x.NegocioID,
                        principalTable: "Negocios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalariosEmpleado",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    Periodicidad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalariosEmpleado", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SalariosEmpleado_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_NegocioID_Codigo",
                table: "Empleados",
                columns: new[] { "NegocioID", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalariosEmpleado_EmpleadoID",
                table: "SalariosEmpleado",
                column: "EmpleadoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalariosEmpleado");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Negocios");
        }
    }
}
