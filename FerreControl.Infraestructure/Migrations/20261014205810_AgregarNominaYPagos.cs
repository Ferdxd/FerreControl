using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNominaYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaIngreso",
                table: "Empleados",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Nominas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NegocioID = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicioPeriodo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFinPeriodo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nominas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Nominas_Negocios_NegocioID",
                        column: x => x.NegocioID,
                        principalTable: "Negocios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NominasDetalle",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NominaID = table.Column<int>(type: "INTEGER", nullable: false),
                    EmpleadoID = table.Column<int>(type: "INTEGER", nullable: false),
                    SalarioEmpleadoID = table.Column<int>(type: "INTEGER", nullable: true),
                    SalarioBaseCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    BonificacionesCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    HorasExtraCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    DescuentosCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalPagarCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasDetalle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NominasDetalle_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NominasDetalle_Nominas_NominaID",
                        column: x => x.NominaID,
                        principalTable: "Nominas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NominasDetalle_SalariosEmpleado_SalarioEmpleadoID",
                        column: x => x.SalarioEmpleadoID,
                        principalTable: "SalariosEmpleado",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PagosNomina",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NominaDetalleID = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosNomina", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PagosNomina_NominasDetalle_NominaDetalleID",
                        column: x => x.NominaDetalleID,
                        principalTable: "NominasDetalle",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_NegocioID",
                table: "Nominas",
                column: "NegocioID");

            migrationBuilder.CreateIndex(
                name: "IX_NominasDetalle_EmpleadoID",
                table: "NominasDetalle",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_NominasDetalle_NominaID_EmpleadoID",
                table: "NominasDetalle",
                columns: new[] { "NominaID", "EmpleadoID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NominasDetalle_SalarioEmpleadoID",
                table: "NominasDetalle",
                column: "SalarioEmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_PagosNomina_NominaDetalleID",
                table: "PagosNomina",
                column: "NominaDetalleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosNomina");

            migrationBuilder.DropTable(
                name: "NominasDetalle");

            migrationBuilder.DropTable(
                name: "Nominas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaIngreso",
                table: "Empleados",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
