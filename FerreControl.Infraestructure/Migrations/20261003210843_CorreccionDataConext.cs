using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionDataConext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadoCredencial_Empleados_EmpleadoID",
                table: "EmpleadoCredencial");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionAsistencia_EmpleadoCredencial_EmpleadoCredencialID",
                table: "MarcacionAsistencia");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionAsistencia_EmpleadosHorarios_EmpleadoHorarioID",
                table: "MarcacionAsistencia");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionAsistencia_Empleados_EmpleadoID",
                table: "MarcacionAsistencia");

            migrationBuilder.DropIndex(
                name: "IX_EmpleadosHorarios_EmpleadoID",
                table: "EmpleadosHorarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarcacionAsistencia",
                table: "MarcacionAsistencia");

            migrationBuilder.DropIndex(
                name: "IX_MarcacionAsistencia_EmpleadoID",
                table: "MarcacionAsistencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpleadoCredencial",
                table: "EmpleadoCredencial");

            migrationBuilder.RenameTable(
                name: "MarcacionAsistencia",
                newName: "MarcacionesAsistencia");

            migrationBuilder.RenameTable(
                name: "EmpleadoCredencial",
                newName: "EmpleadosCredenciales");

            migrationBuilder.RenameIndex(
                name: "IX_MarcacionAsistencia_EmpleadoHorarioID",
                table: "MarcacionesAsistencia",
                newName: "IX_MarcacionesAsistencia_EmpleadoHorarioID");

            migrationBuilder.RenameIndex(
                name: "IX_MarcacionAsistencia_EmpleadoCredencialID",
                table: "MarcacionesAsistencia",
                newName: "IX_MarcacionesAsistencia_EmpleadoCredencialID");

            migrationBuilder.RenameIndex(
                name: "IX_EmpleadoCredencial_EmpleadoID",
                table: "EmpleadosCredenciales",
                newName: "IX_EmpleadosCredenciales_EmpleadoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarcacionesAsistencia",
                table: "MarcacionesAsistencia",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpleadosCredenciales",
                table: "EmpleadosCredenciales",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosHorarios_EmpleadoID_FechaInicio",
                table: "EmpleadosHorarios",
                columns: new[] { "EmpleadoID", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_MarcacionesAsistencia_EmpleadoID_FechaHora",
                table: "MarcacionesAsistencia",
                columns: new[] { "EmpleadoID", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosCredenciales_TipoCredencial_Codigo",
                table: "EmpleadosCredenciales",
                columns: new[] { "TipoCredencial", "Codigo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadosCredenciales_Empleados_EmpleadoID",
                table: "EmpleadosCredenciales",
                column: "EmpleadoID",
                principalTable: "Empleados",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionesAsistencia_EmpleadosCredenciales_EmpleadoCredencialID",
                table: "MarcacionesAsistencia",
                column: "EmpleadoCredencialID",
                principalTable: "EmpleadosCredenciales",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionesAsistencia_EmpleadosHorarios_EmpleadoHorarioID",
                table: "MarcacionesAsistencia",
                column: "EmpleadoHorarioID",
                principalTable: "EmpleadosHorarios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionesAsistencia_Empleados_EmpleadoID",
                table: "MarcacionesAsistencia",
                column: "EmpleadoID",
                principalTable: "Empleados",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpleadosCredenciales_Empleados_EmpleadoID",
                table: "EmpleadosCredenciales");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionesAsistencia_EmpleadosCredenciales_EmpleadoCredencialID",
                table: "MarcacionesAsistencia");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionesAsistencia_EmpleadosHorarios_EmpleadoHorarioID",
                table: "MarcacionesAsistencia");

            migrationBuilder.DropForeignKey(
                name: "FK_MarcacionesAsistencia_Empleados_EmpleadoID",
                table: "MarcacionesAsistencia");

            migrationBuilder.DropIndex(
                name: "IX_EmpleadosHorarios_EmpleadoID_FechaInicio",
                table: "EmpleadosHorarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarcacionesAsistencia",
                table: "MarcacionesAsistencia");

            migrationBuilder.DropIndex(
                name: "IX_MarcacionesAsistencia_EmpleadoID_FechaHora",
                table: "MarcacionesAsistencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpleadosCredenciales",
                table: "EmpleadosCredenciales");

            migrationBuilder.DropIndex(
                name: "IX_EmpleadosCredenciales_TipoCredencial_Codigo",
                table: "EmpleadosCredenciales");

            migrationBuilder.RenameTable(
                name: "MarcacionesAsistencia",
                newName: "MarcacionAsistencia");

            migrationBuilder.RenameTable(
                name: "EmpleadosCredenciales",
                newName: "EmpleadoCredencial");

            migrationBuilder.RenameIndex(
                name: "IX_MarcacionesAsistencia_EmpleadoHorarioID",
                table: "MarcacionAsistencia",
                newName: "IX_MarcacionAsistencia_EmpleadoHorarioID");

            migrationBuilder.RenameIndex(
                name: "IX_MarcacionesAsistencia_EmpleadoCredencialID",
                table: "MarcacionAsistencia",
                newName: "IX_MarcacionAsistencia_EmpleadoCredencialID");

            migrationBuilder.RenameIndex(
                name: "IX_EmpleadosCredenciales_EmpleadoID",
                table: "EmpleadoCredencial",
                newName: "IX_EmpleadoCredencial_EmpleadoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarcacionAsistencia",
                table: "MarcacionAsistencia",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpleadoCredencial",
                table: "EmpleadoCredencial",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosHorarios_EmpleadoID",
                table: "EmpleadosHorarios",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_MarcacionAsistencia_EmpleadoID",
                table: "MarcacionAsistencia",
                column: "EmpleadoID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpleadoCredencial_Empleados_EmpleadoID",
                table: "EmpleadoCredencial",
                column: "EmpleadoID",
                principalTable: "Empleados",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionAsistencia_EmpleadoCredencial_EmpleadoCredencialID",
                table: "MarcacionAsistencia",
                column: "EmpleadoCredencialID",
                principalTable: "EmpleadoCredencial",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionAsistencia_EmpleadosHorarios_EmpleadoHorarioID",
                table: "MarcacionAsistencia",
                column: "EmpleadoHorarioID",
                principalTable: "EmpleadosHorarios",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MarcacionAsistencia_Empleados_EmpleadoID",
                table: "MarcacionAsistencia",
                column: "EmpleadoID",
                principalTable: "Empleados",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
