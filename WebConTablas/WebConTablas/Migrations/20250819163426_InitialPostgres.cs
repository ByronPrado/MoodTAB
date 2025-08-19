using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    ID_Pregunta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    Extra = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OpcionesSeleccion = table.Column<string>(type: "text", nullable: true),
                    EscalaMin = table.Column<int>(type: "integer", nullable: true),
                    EscalaMax = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.ID_Pregunta);
                });

            migrationBuilder.CreateTable(
                name: "Psiquiatras",
                columns: table => new
                {
                    ID_Psiquiatra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psiquiatras", x => x.ID_Psiquiatra);
                });

            migrationBuilder.CreateTable(
                name: "Formularios",
                columns: table => new
                {
                    ID_Formulario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Psiquiatra = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formularios", x => x.ID_Formulario);
                    table.ForeignKey(
                        name: "FK_Formularios_Psiquiatras_ID_Psiquiatra",
                        column: x => x.ID_Psiquiatra,
                        principalTable: "Psiquiatras",
                        principalColumn: "ID_Psiquiatra",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    ID_Paciente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Diagnostico = table.Column<string>(type: "text", nullable: true),
                    Edad = table.Column<int>(type: "integer", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    ID_Psiquiatra = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.ID_Paciente);
                    table.ForeignKey(
                        name: "FK_Pacientes_Psiquiatras_ID_Psiquiatra",
                        column: x => x.ID_Psiquiatra,
                        principalTable: "Psiquiatras",
                        principalColumn: "ID_Psiquiatra",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormularioPreguntas",
                columns: table => new
                {
                    ID_Formulario = table.Column<int>(type: "integer", nullable: false),
                    ID_Pregunta = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormularioPreguntas", x => new { x.ID_Formulario, x.ID_Pregunta });
                    table.ForeignKey(
                        name: "FK_FormularioPreguntas_Formularios_ID_Formulario",
                        column: x => x.ID_Formulario,
                        principalTable: "Formularios",
                        principalColumn: "ID_Formulario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormularioPreguntas_Preguntas_ID_Pregunta",
                        column: x => x.ID_Pregunta,
                        principalTable: "Preguntas",
                        principalColumn: "ID_Pregunta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiariosEmocionales",
                columns: table => new
                {
                    ID_Diario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Paciente = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Emociones = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Pasos = table.Column<int>(type: "integer", nullable: true),
                    Horas_celular = table.Column<int>(type: "integer", nullable: true),
                    Horas_redes = table.Column<int>(type: "integer", nullable: true),
                    Hora_dormida = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiariosEmocionales", x => x.ID_Diario);
                    table.ForeignKey(
                        name: "FK_DiariosEmocionales_Pacientes_ID_Paciente",
                        column: x => x.ID_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "ID_Paciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormulariosAsignados",
                columns: table => new
                {
                    ID_Asignacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Formulario = table.Column<int>(type: "integer", nullable: false),
                    ID_Paciente = table.Column<int>(type: "integer", nullable: false),
                    Fecha_Asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fecha_Limite = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormulariosAsignados", x => x.ID_Asignacion);
                    table.ForeignKey(
                        name: "FK_FormulariosAsignados_Formularios_ID_Formulario",
                        column: x => x.ID_Formulario,
                        principalTable: "Formularios",
                        principalColumn: "ID_Formulario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormulariosAsignados_Pacientes_ID_Paciente",
                        column: x => x.ID_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "ID_Paciente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    ID_Respuesta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_Asignacion = table.Column<int>(type: "integer", nullable: false),
                    ID_Pregunta = table.Column<int>(type: "integer", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    Fecha_Respuesta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.ID_Respuesta);
                    table.ForeignKey(
                        name: "FK_Respuestas_FormulariosAsignados_ID_Asignacion",
                        column: x => x.ID_Asignacion,
                        principalTable: "FormulariosAsignados",
                        principalColumn: "ID_Asignacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Respuestas_Preguntas_ID_Pregunta",
                        column: x => x.ID_Pregunta,
                        principalTable: "Preguntas",
                        principalColumn: "ID_Pregunta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Preguntas",
                columns: new[] { "ID_Pregunta", "Contenido", "Created_at", "Edited_at", "EscalaMax", "EscalaMin", "Extra", "OpcionesSeleccion", "Tipo" },
                values: new object[] { 1, "¿Cómo te has sentido hoy?", new DateTime(2024, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, "texto" });

            migrationBuilder.InsertData(
                table: "Psiquiatras",
                columns: new[] { "ID_Psiquiatra", "Contrasena", "Email", "Nombre", "Telefono" },
                values: new object[] { 1, "1234", "juan@ejemplo.com", "Dr. Juan Pérez", "555-1234" });

            migrationBuilder.InsertData(
                table: "Formularios",
                columns: new[] { "ID_Formulario", "Created_at", "Descripcion", "ID_Psiquiatra", "Titulo" },
                values: new object[] { 1, new DateTime(2024, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Formulario para evaluar estado inicial del paciente", 1, "Evaluación inicial" });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "ID_Paciente", "Diagnostico", "Edad", "Email", "ID_Psiquiatra", "Nombre", "Sexo", "Telefono" },
                values: new object[] { 1, "Ansiedad", 30, "ana@mail.com", 1, "Ana Gómez", "F", "555-5678" });

            migrationBuilder.InsertData(
                table: "DiariosEmocionales",
                columns: new[] { "ID_Diario", "Descripcion", "Emociones", "Estado", "Fecha", "Hora_dormida", "Horas_celular", "Horas_redes", "ID_Paciente", "Pasos" },
                values: new object[] { 1, "Tuve un día difícil", "{\"feliz\":0,\"triste\":1}", "inhibido", new DateTime(2024, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), "23:00", 4, 2, 1, 3000 });

            migrationBuilder.InsertData(
                table: "FormularioPreguntas",
                columns: new[] { "ID_Formulario", "ID_Pregunta", "Orden" },
                values: new object[] { 1, 1, 1 });

            migrationBuilder.InsertData(
                table: "FormulariosAsignados",
                columns: new[] { "ID_Asignacion", "Estado", "Fecha_Asignacion", "Fecha_Limite", "ID_Formulario", "ID_Paciente" },
                values: new object[] { 1, "pendiente", new DateTime(2024, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 6, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_DiariosEmocionales_ID_Paciente_Fecha",
                table: "DiariosEmocionales",
                columns: new[] { "ID_Paciente", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormularioPreguntas_ID_Pregunta",
                table: "FormularioPreguntas",
                column: "ID_Pregunta");

            migrationBuilder.CreateIndex(
                name: "IX_Formularios_ID_Psiquiatra",
                table: "Formularios",
                column: "ID_Psiquiatra");

            migrationBuilder.CreateIndex(
                name: "IX_FormulariosAsignados_ID_Formulario",
                table: "FormulariosAsignados",
                column: "ID_Formulario");

            migrationBuilder.CreateIndex(
                name: "IX_FormulariosAsignados_ID_Paciente",
                table: "FormulariosAsignados",
                column: "ID_Paciente");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_ID_Psiquiatra",
                table: "Pacientes",
                column: "ID_Psiquiatra");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_ID_Asignacion_ID_Pregunta",
                table: "Respuestas",
                columns: new[] { "ID_Asignacion", "ID_Pregunta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_ID_Pregunta",
                table: "Respuestas",
                column: "ID_Pregunta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiariosEmocionales");

            migrationBuilder.DropTable(
                name: "FormularioPreguntas");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "FormulariosAsignados");

            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "Formularios");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Psiquiatras");
        }
    }
}
