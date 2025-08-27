using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class Parientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComentariosExternos",
                columns: table => new
                {
                    ID_ComentarioExterno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ID_UsuarioExterno = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosExternos", x => x.ID_ComentarioExterno);
                    table.ForeignKey(
                        name: "FK_ComentariosExternos_UsuariosExternos_ID_UsuarioExterno",
                        column: x => x.ID_UsuarioExterno,
                        principalTable: "UsuariosExternos",
                        principalColumn: "ID_UsuarioExterno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosExternos_ID_UsuarioExterno",
                table: "ComentariosExternos",
                column: "ID_UsuarioExterno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentariosExternos");
        }
    }
}
