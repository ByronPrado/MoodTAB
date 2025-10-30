using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class PacienteSeveridad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Severidad",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "ID_Paciente",
                keyValue: 1,
                column: "Severidad",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severidad",
                table: "Pacientes");
        }
    }
}
