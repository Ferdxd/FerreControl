using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerreControl.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposCalculoNomina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasProgramados",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiasTrabajados",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinutosExtra",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinutosTrabajados",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "SalarioDevengadoCentavos",
                table: "NominasDetalle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasProgramados",
                table: "NominasDetalle");

            migrationBuilder.DropColumn(
                name: "DiasTrabajados",
                table: "NominasDetalle");

            migrationBuilder.DropColumn(
                name: "MinutosExtra",
                table: "NominasDetalle");

            migrationBuilder.DropColumn(
                name: "MinutosTrabajados",
                table: "NominasDetalle");

            migrationBuilder.DropColumn(
                name: "SalarioDevengadoCentavos",
                table: "NominasDetalle");
        }
    }
}
