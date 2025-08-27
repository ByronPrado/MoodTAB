using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class RenameID_UsuarioExternoToIdUsuarioExterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosExternos_UsuariosExternos_ID_UsuarioExterno",
                table: "ComentariosExternos");

            migrationBuilder.RenameColumn(
                name: "ID_UsuarioExterno",
                table: "UsuariosExternos",
                newName: "IdUsuarioExterno");

            migrationBuilder.RenameColumn(
                name: "ID_UsuarioExterno",
                table: "ComentariosExternos",
                newName: "IdUsuarioExterno");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosExternos_ID_UsuarioExterno",
                table: "ComentariosExternos",
                newName: "IX_ComentariosExternos_IdUsuarioExterno");

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosExternos_UsuariosExternos_IdUsuarioExterno",
                table: "ComentariosExternos",
                column: "IdUsuarioExterno",
                principalTable: "UsuariosExternos",
                principalColumn: "IdUsuarioExterno",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosExternos_UsuariosExternos_IdUsuarioExterno",
                table: "ComentariosExternos");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioExterno",
                table: "UsuariosExternos",
                newName: "ID_UsuarioExterno");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioExterno",
                table: "ComentariosExternos",
                newName: "ID_UsuarioExterno");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosExternos_IdUsuarioExterno",
                table: "ComentariosExternos",
                newName: "IX_ComentariosExternos_ID_UsuarioExterno");

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosExternos_UsuariosExternos_ID_UsuarioExterno",
                table: "ComentariosExternos",
                column: "ID_UsuarioExterno",
                principalTable: "UsuariosExternos",
                principalColumn: "ID_UsuarioExterno",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
