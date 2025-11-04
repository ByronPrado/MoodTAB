using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class CuestionarioVisto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Ultima_Revision_Psiquiatra",
                table: "FormulariosAsignados",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FormulariosAsignados",
                keyColumn: "ID_Asignacion",
                keyValue: 1,
                column: "Ultima_Revision_Psiquiatra",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ultima_Revision_Psiquiatra",
                table: "FormulariosAsignados");
        }
    }
}
