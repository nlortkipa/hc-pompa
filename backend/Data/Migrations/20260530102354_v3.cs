using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmsCodes_UserId",
                table: "SmsCodes");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "users",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "users",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PersonalId",
                table: "users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName", "PersonalId" },
                values: new object[] { "Nikoloz", "Lortkipanidze", "12345678901" });

            migrationBuilder.CreateIndex(
                name: "IX_users_PhoneNumber",
                table: "users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsCodes_UserId",
                table: "SmsCodes",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_PhoneNumber",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_SmsCodes_UserId",
                table: "SmsCodes");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PersonalId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "users",
                newName: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCodes_UserId",
                table: "SmsCodes",
                column: "UserId");
        }
    }
}
