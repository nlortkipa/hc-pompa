using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddStationaryPlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStationary",
                table: "Plates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsStationary",
                value: true);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsStationary",
                value: true);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsStationary",
                value: false);

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsStationary",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStationary",
                table: "Plates");
        }
    }
}
