using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CreacionEmpleadoCredencialesYMarcaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpleadoCredencial",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoCredencial = table.Column<string>(type: "TEXT", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoCredencial", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmpleadoCredencial_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarcacionAsistencia",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    EmpleadoCredencialID = table.Column<int>(type: "INTEGER", nullable: true),
                    EmpleadoHorarioID = table.Column<int>(type: "INTEGER", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoMarcacion = table.Column<string>(type: "TEXT", nullable: false),
                    MetodoMarcacion = table.Column<string>(type: "TEXT", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcacionAsistencia", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MarcacionAsistencia_EmpleadoCredencial_EmpleadoCredencialID",
                        column: x => x.EmpleadoCredencialID,
                        principalTable: "EmpleadoCredencial",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MarcacionAsistencia_EmpleadosHorarios_EmpleadoHorarioID",
                        column: x => x.EmpleadoHorarioID,
                        principalTable: "EmpleadosHorarios",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MarcacionAsistencia_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoCredencial_EmpleadoID",
                table: "EmpleadoCredencial",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_MarcacionAsistencia_EmpleadoCredencialID",
                table: "MarcacionAsistencia",
                column: "EmpleadoCredencialID");

            migrationBuilder.CreateIndex(
                name: "IX_MarcacionAsistencia_EmpleadoHorarioID",
                table: "MarcacionAsistencia",
                column: "EmpleadoHorarioID");

            migrationBuilder.CreateIndex(
                name: "IX_MarcacionAsistencia_EmpleadoID",
                table: "MarcacionAsistencia",
                column: "EmpleadoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarcacionAsistencia");

            migrationBuilder.DropTable(
                name: "EmpleadoCredencial");
        }
    }
}
