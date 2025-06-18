using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class PreguntaOpcionesYRangonuevocambio5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EscalaMax",
                table: "Preguntas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalaMin",
                table: "Preguntas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpcionesSeleccion",
                table: "Preguntas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Preguntas",
                keyColumn: "ID_Pregunta",
                keyValue: 1,
                columns: new[] { "EscalaMax", "EscalaMin", "OpcionesSeleccion" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalaMax",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "EscalaMin",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "OpcionesSeleccion",
                table: "Preguntas");
        }
    }
}
