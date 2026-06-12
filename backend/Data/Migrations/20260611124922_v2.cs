using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PumpOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    ConcreteVolume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PumpRelocation = table.Column<int>(type: "int", nullable: false),
                    StationaryRelocation = table.Column<int>(type: "int", nullable: false),
                    HydraulicRelocation = table.Column<int>(type: "int", nullable: false),
                    StationaryReposition = table.Column<int>(type: "int", nullable: false),
                    IdleTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DismantleAssemble = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumpOperations_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpOperations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpOperations_users_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperations_AddressId",
                table: "PumpOperations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperations_CompanyId",
                table: "PumpOperations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperations_OperatorId",
                table: "PumpOperations",
                column: "OperatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumpOperations");
        }
    }
}
