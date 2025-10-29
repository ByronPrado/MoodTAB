using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebConTablas.Migrations
{
    /// <inheritdoc />
    public partial class StartSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""DiariosEmocionales"" 
                ALTER COLUMN ""Hora_dormida"" TYPE real 
                USING ""Hora_dormida""::real;
            ");

            migrationBuilder.AddColumn<float>(
                name: "Coherencia",
                table: "DiariosEmocionales",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Errores_gramaticales",
                table: "DiariosEmocionales",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HRV_VariabilidadFrecuencia",
                table: "DiariosEmocionales",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HR_RitmoCardiaco",
                table: "DiariosEmocionales",
                type: "real",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DiariosEmocionales",
                keyColumn: "ID_Diario",
                keyValue: 1,
                columns: new[] { "Coherencia", "Errores_gramaticales", "HRV_VariabilidadFrecuencia", "HR_RitmoCardiaco", "Hora_dormida" },
                values: new object[] { null, null, null, null, 7.5f });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
