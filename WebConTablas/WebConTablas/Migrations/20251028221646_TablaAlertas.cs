using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class TablaAlertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Hora_dormida",
                table: "DiariosEmocionales",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    ID_Alerta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Paciente = table.Column<int>(type: "integer", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.ID_Alerta);
                    table.ForeignKey(
                        name: "FK_Alertas_Pacientes_ID_Paciente",
                        column: x => x.ID_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "ID_Paciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alertas",
                columns: new[] { "ID_Alerta", "Contenido", "Created_at", "Estado", "ID_Paciente", "Tipo" },
                values: new object[] { 1, "Desvio diario emocional", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "No Visto", 1, "Desvio" });

            migrationBuilder.UpdateData(
                table: "DiariosEmocionales",
                keyColumn: "ID_Diario",
                keyValue: 1,
                columns: new[] { "Coherencia", "Errores_gramaticales", "HRV_VariabilidadFrecuencia", "HR_RitmoCardiaco", "Hora_dormida" },
                values: new object[] { null, null, null, null, 7.5f });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_ID_Paciente",
                table: "Alertas",
                column: "ID_Paciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropColumn(
                name: "Coherencia",
                table: "DiariosEmocionales");

            migrationBuilder.DropColumn(
                name: "Errores_gramaticales",
                table: "DiariosEmocionales");

            migrationBuilder.DropColumn(
                name: "HRV_VariabilidadFrecuencia",
                table: "DiariosEmocionales");

            migrationBuilder.DropColumn(
                name: "HR_RitmoCardiaco",
                table: "DiariosEmocionales");

            migrationBuilder.AlterColumn<string>(
                name: "Hora_dormida",
                table: "DiariosEmocionales",
                type: "text",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "DiariosEmocionales",
                keyColumn: "ID_Diario",
                keyValue: 1,
                column: "Hora_dormida",
                value: "23:00");
        }
    }
}
