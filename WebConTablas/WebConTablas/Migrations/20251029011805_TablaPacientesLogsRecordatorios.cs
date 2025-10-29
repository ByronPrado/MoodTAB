using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class TablaPacientesLogsRecordatorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Pacientes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    ID_Log = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Paciente = table.Column<int>(type: "integer", nullable: false),
                    ID_Psiquiatra = table.Column<int>(type: "integer", nullable: false),
                    TipoLog = table.Column<string>(type: "text", nullable: false),
                    Actual = table.Column<string>(type: "text", nullable: true),
                    Anterior = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.ID_Log);
                    table.ForeignKey(
                        name: "FK_Logs_Pacientes_ID_Paciente",
                        column: x => x.ID_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "ID_Paciente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Logs_Psiquiatras_ID_Psiquiatra",
                        column: x => x.ID_Psiquiatra,
                        principalTable: "Psiquiatras",
                        principalColumn: "ID_Psiquiatra",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordatoriosPsiquiatra",
                columns: table => new
                {
                    ID_RecordatorioPsiquiatra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Psiquiatra = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Grupo = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordatoriosPsiquiatra", x => x.ID_RecordatorioPsiquiatra);
                    table.ForeignKey(
                        name: "FK_RecordatoriosPsiquiatra_Psiquiatras_ID_Psiquiatra",
                        column: x => x.ID_Psiquiatra,
                        principalTable: "Psiquiatras",
                        principalColumn: "ID_Psiquiatra",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "ID_Paciente",
                keyValue: 1,
                column: "Contrasena",
                value: "1234");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ID_Paciente",
                table: "Logs",
                column: "ID_Paciente");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ID_Psiquiatra",
                table: "Logs",
                column: "ID_Psiquiatra");

            migrationBuilder.CreateIndex(
                name: "IX_RecordatoriosPsiquiatra_ID_Psiquiatra",
                table: "RecordatoriosPsiquiatra",
                column: "ID_Psiquiatra");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "RecordatoriosPsiquiatra");

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Pacientes");
        }
    }
}
