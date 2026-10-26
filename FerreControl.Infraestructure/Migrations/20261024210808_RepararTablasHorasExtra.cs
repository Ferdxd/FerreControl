using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    public partial class RepararTablasHorasExtra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REGLAS DE HORAS EXTRA
            // =====================================================

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "ReglasHoraExtra"
                (
                    "ID" INTEGER NOT NULL
                        CONSTRAINT "PK_ReglasHoraExtra"
                        PRIMARY KEY AUTOINCREMENT,

                    "NegocioID" INTEGER NOT NULL,

                    "Nombre" TEXT NOT NULL,

                    "TarifaHoraCentavos" INTEGER NOT NULL,

                    "TipoFraccion" TEXT NOT NULL,

                    "RequiereAprobacion" INTEGER NOT NULL,

                    "CuentaEntradaAnticipada" INTEGER NOT NULL,

                    "Estado" INTEGER NOT NULL,

                    "FechaCreacion" TEXT NOT NULL,

                    CONSTRAINT "FK_ReglasHoraExtra_Negocios_NegocioID"
                        FOREIGN KEY ("NegocioID")
                        REFERENCES "Negocios" ("ID")
                        ON DELETE RESTRICT
                );
                """);


            // =====================================================
            // REGLA DE HORAS EXTRA DEL EMPLEADO
            // =====================================================

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "EmpleadosReglasHoraExtra"
                (
                    "ID" INTEGER NOT NULL
                        CONSTRAINT "PK_EmpleadosReglasHoraExtra"
                        PRIMARY KEY AUTOINCREMENT,

                    "EmpleadoID" INTEGER NOT NULL,

                    "ReglaHoraExtraID" INTEGER NOT NULL,

                    "FechaInicio" TEXT NOT NULL,

                    "FechaFin" TEXT NULL,

                    "Estado" INTEGER NOT NULL,

                    "FechaCreacion" TEXT NOT NULL,

                    CONSTRAINT "FK_EmpleadosReglasHoraExtra_Empleados_EmpleadoID"
                        FOREIGN KEY ("EmpleadoID")
                        REFERENCES "Empleados" ("ID")
                        ON DELETE RESTRICT,

                    CONSTRAINT "FK_EmpleadosReglasHoraExtra_ReglasHoraExtra_ReglaHoraExtraID"
                        FOREIGN KEY ("ReglaHoraExtraID")
                        REFERENCES "ReglasHoraExtra" ("ID")
                        ON DELETE RESTRICT
                );
                """);


            // =====================================================
            // REGISTROS DE HORAS EXTRA
            // =====================================================

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "RegistrosHoraExtra"
                (
                    "ID" INTEGER NOT NULL
                        CONSTRAINT "PK_RegistrosHoraExtra"
                        PRIMARY KEY AUTOINCREMENT,

                    "EmpleadoID" INTEGER NOT NULL,

                    "EmpleadoReglaHoraExtraID" INTEGER NULL,

                    "Fecha" TEXT NOT NULL,

                    "MinutosCalculados" INTEGER NOT NULL,

                    "MinutosAprobados" INTEGER NOT NULL,

                    "TarifaHoraCentavos" INTEGER NOT NULL,

                    "MontoCentavos" INTEGER NOT NULL,

                    "Estado" TEXT NOT NULL,

                    "ExcedeLimite12Horas" INTEGER NOT NULL,

                    "Observacion" TEXT NULL,

                    "FechaCalculo" TEXT NOT NULL,

                    "FechaDecision" TEXT NULL,

                    CONSTRAINT "FK_RegistrosHoraExtra_Empleados_EmpleadoID"
                        FOREIGN KEY ("EmpleadoID")
                        REFERENCES "Empleados" ("ID")
                        ON DELETE RESTRICT,

                    CONSTRAINT "FK_RegistrosHoraExtra_EmpleadosReglasHoraExtra_EmpleadoReglaHoraExtraID"
                        FOREIGN KEY ("EmpleadoReglaHoraExtraID")
                        REFERENCES "EmpleadosReglasHoraExtra" ("ID")
                        ON DELETE RESTRICT
                );
                """);


            // =====================================================
            // ÍNDICES
            // =====================================================

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS
                "IX_ReglasHoraExtra_NegocioID_Nombre"
                ON "ReglasHoraExtra"
                (
                    "NegocioID",
                    "Nombre"
                );
                """);


            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS
                "IX_EmpleadosReglasHoraExtra_EmpleadoID_FechaInicio"
                ON "EmpleadosReglasHoraExtra"
                (
                    "EmpleadoID",
                    "FechaInicio"
                );
                """);


            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS
                "IX_EmpleadosReglasHoraExtra_ReglaHoraExtraID"
                ON "EmpleadosReglasHoraExtra"
                (
                    "ReglaHoraExtraID"
                );
                """);


            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS
                "IX_RegistrosHoraExtra_EmpleadoID_Fecha"
                ON "RegistrosHoraExtra"
                (
                    "EmpleadoID",
                    "Fecha"
                );
                """);


            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS
                "IX_RegistrosHoraExtra_EmpleadoReglaHoraExtraID"
                ON "RegistrosHoraExtra"
                (
                    "EmpleadoReglaHoraExtraID"
                );
                """);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TABLE IF EXISTS "RegistrosHoraExtra";
                """);

            migrationBuilder.Sql(
                """
                DROP TABLE IF EXISTS "EmpleadosReglasHoraExtra";
                """);

            migrationBuilder.Sql(
                """
                DROP TABLE IF EXISTS "ReglasHoraExtra";
                """);
        }
    }
}