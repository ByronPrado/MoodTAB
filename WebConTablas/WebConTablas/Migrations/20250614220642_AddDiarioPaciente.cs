using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class AddDiarioPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PacientePreguntas");

            migrationBuilder.CreateTable(
                name: "DiariosPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPaciente = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Pasos = table.Column<string>(type: "TEXT", nullable: false),
                    HorasCelular = table.Column<double>(type: "REAL", nullable: false),
                    DesbloqueosCelular = table.Column<int>(type: "INTEGER", nullable: false),
                    HorasRedesSociales = table.Column<double>(type: "REAL", nullable: false),
                    HorasSueno = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiariosPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiariosPacientes_Pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiariosPacientes_IdPaciente",
                table: "DiariosPacientes",
                column: "IdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiariosPacientes");

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
    }
}
