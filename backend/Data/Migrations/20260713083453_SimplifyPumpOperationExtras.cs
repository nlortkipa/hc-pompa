using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyPumpOperationExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HydraulicRelocation",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "PumpRelocation",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "StationaryRelocation",
                table: "PumpOperations");

            migrationBuilder.RenameColumn(
                name: "StationaryReposition",
                table: "PumpOperations",
                newName: "AdditionalStanding");

            migrationBuilder.RenameColumn(
                name: "IdleTime",
                table: "PumpOperations",
                newName: "IsIdle");

            migrationBuilder.AlterColumn<bool>(
                name: "IsIdle",
                table: "PumpOperations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalStanding",
                table: "PumpOperations",
                newName: "StationaryReposition");

            migrationBuilder.RenameColumn(
                name: "IsIdle",
                table: "PumpOperations",
                newName: "IdleTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "IdleTime",
                table: "PumpOperations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "HydraulicRelocation",
                table: "PumpOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PumpRelocation",
                table: "PumpOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StationaryRelocation",
                table: "PumpOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
