using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class PacientePlanSeguro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanSeguro",
                table: "Pacientes",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "ID_Paciente",
                keyValue: 1,
                column: "PlanSeguro",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanSeguro",
                table: "Pacientes");
        }
    }
}
