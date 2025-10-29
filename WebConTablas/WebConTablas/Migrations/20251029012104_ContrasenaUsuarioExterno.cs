using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class ContrasenaUsuarioExterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "UsuariosExternos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "UsuariosExternos");
        }
    }
}
