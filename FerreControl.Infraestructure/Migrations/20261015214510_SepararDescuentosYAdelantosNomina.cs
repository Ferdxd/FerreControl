using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class SepararDescuentosYAdelantosNomina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DescuentosCentavos",
                table: "NominasDetalle",
                newName: "OtrosDescuentosCentavos");

            migrationBuilder.AddColumn<long>(
                name: "AdelantosCentavos",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AdelantosEmpleado",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaAdelanto = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetodoEntrega = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdelantosEmpleado", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdelantosEmpleado_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdelantosAplicaciones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdelantoEmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    NominaDetalleID = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdelantosAplicaciones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdelantosAplicaciones_AdelantosEmpleado_AdelantoEmpleadoID",
                        column: x => x.AdelantoEmpleadoID,
                        principalTable: "AdelantosEmpleado",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdelantosAplicaciones_NominasDetalle_NominaDetalleID",
                        column: x => x.NominaDetalleID,
                        principalTable: "NominasDetalle",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdelantosAplicaciones_AdelantoEmpleadoID_NominaDetalleID",
                table: "AdelantosAplicaciones",
                columns: new[] { "AdelantoEmpleadoID", "NominaDetalleID" });

            migrationBuilder.CreateIndex(
                name: "IX_AdelantosAplicaciones_NominaDetalleID",
                table: "AdelantosAplicaciones",
                column: "NominaDetalleID");

            migrationBuilder.CreateIndex(
                name: "IX_AdelantosEmpleado_EmpleadoID_FechaAdelanto",
                table: "AdelantosEmpleado",
                columns: new[] { "EmpleadoID", "FechaAdelanto" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdelantosAplicaciones");

            migrationBuilder.DropTable(
                name: "AdelantosEmpleado");

            migrationBuilder.DropColumn(
                name: "AdelantosCentavos",
                table: "NominasDetalle");

            migrationBuilder.RenameColumn(
                name: "OtrosDescuentosCentavos",
                table: "NominasDetalle",
                newName: "DescuentosCentavos");
        }
    }
}
