using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class AgregarContenidoAPregunta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PacientePreguntas");

            migrationBuilder.RenameColumn(
                name: "Texto",
                table: "Preguntas",
                newName: "Contenido");

            migrationBuilder.AddColumn<int>(
                name: "IdPaciente",
                table: "Preguntas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_IdPaciente",
                table: "Preguntas",
                column: "IdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_Preguntas_Pacientes_IdPaciente",
                table: "Preguntas",
                column: "IdPaciente",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preguntas_Pacientes_IdPaciente",
                table: "Preguntas");

            migrationBuilder.DropIndex(
                name: "IX_Preguntas_IdPaciente",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "IdPaciente",
                table: "Preguntas");

            migrationBuilder.RenameColumn(
                name: "Contenido",
                table: "Preguntas",
                newName: "Texto");

            migrationBuilder.CreateTable(
                name: "PacientePreguntas",
                columns: table => new
                {
                    PacienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientePreguntas", x => new { x.PacienteId, x.PreguntaId });
                    table.ForeignKey(
                        name: "FK_PacientePreguntas_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PacientePreguntas_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PacientePreguntas_PreguntaId",
                table: "PacientePreguntas",
                column: "PreguntaId");
        }
    }
}
