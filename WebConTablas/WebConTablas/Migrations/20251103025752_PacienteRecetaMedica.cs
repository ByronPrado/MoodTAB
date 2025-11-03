using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class PacienteRecetaMedica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecetaMedica",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "ID_Paciente",
                keyValue: 1,
                column: "RecetaMedica",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecetaMedica",
                table: "Pacientes");
        }
    }
}
