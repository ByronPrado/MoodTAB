using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PacientePreguntas",
                columns: table => new
                {
                    PacienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PacientePreguntas");
        }
    }
}
