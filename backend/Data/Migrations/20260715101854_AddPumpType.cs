using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPumpType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PumpType",
                table: "PumpOperations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Plates",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Type",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 7,
                column: "Type",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PumpType",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Plates");
        }
    }
}
