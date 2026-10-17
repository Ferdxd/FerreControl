using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class HorariosAñadidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NegocioID = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ToleranciaMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Horarios_Negocios_NegocioID",
                        column: x => x.NegocioID,
                        principalTable: "Negocios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadosHorarios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioID = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadosHorarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmpleadosHorarios_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpleadosHorarios_Horarios_HorarioID",
                        column: x => x.HorarioID,
                        principalTable: "Horarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HorariosDetalle",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HorarioID = table.Column<int>(type: "INTEGER", nullable: false),
                    DiaSemana = table.Column<int>(type: "INTEGER", nullable: false),
                    EsLaboal = table.Column<bool>(type: "INTEGER", nullable: false),
                    HoraEntrada = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    HoraSalida = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    MinutosDescanso = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosDetalle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HorariosDetalle_Horarios_HorarioID",
                        column: x => x.HorarioID,
                        principalTable: "Horarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosHorarios_EmpleadoID",
                table: "EmpleadosHorarios",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosHorarios_HorarioID",
                table: "EmpleadosHorarios",
                column: "HorarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_NegocioID",
                table: "Horarios",
                column: "NegocioID");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDetalle_HorarioID_DiaSemana",
                table: "HorariosDetalle",
                columns: new[] { "HorarioID", "DiaSemana" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpleadosHorarios");

            migrationBuilder.DropTable(
                name: "HorariosDetalle");

            migrationBuilder.DropTable(
                name: "Horarios");
        }
    }
}
