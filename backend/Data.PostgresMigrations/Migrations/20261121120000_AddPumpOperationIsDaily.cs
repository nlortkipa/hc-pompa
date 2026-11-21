using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPumpOperationIsDaily : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDaily",
                table: "PumpOperations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDaily",
                table: "PumpOperations");
        }
    }
}
