using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Role",
                value: "User");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Role",
                value: "Accountant");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Role",
                value: "Admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Role",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Role",
                value: "User");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Role",
                value: "User");
        }
    }
}
