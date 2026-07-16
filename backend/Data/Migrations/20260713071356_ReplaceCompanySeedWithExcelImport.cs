using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCompanySeedWithExcelImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAddresses_CompanyId",
                table: "CompanyAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_Id",
                table: "Addresses");

            // These ID ranges belong to the former generated CompanyAddressSeedData.
            // Keep any seeded company/address that is already referenced by an operation,
            // and keep any links created later by an administrator.
            migrationBuilder.Sql(
                """
                DELETE FROM [CompanyAddresses]
                WHERE [Id] BETWEEN 1 AND 796;

                DELETE address
                FROM [Addresses] AS address
                WHERE address.[Id] BETWEEN 1 AND 757
                  AND NOT EXISTS (
                      SELECT 1 FROM [PumpOperations] AS operation
                      WHERE operation.[AddressId] = address.[Id])
                  AND NOT EXISTS (
                      SELECT 1 FROM [CompanyAddresses] AS link
                      WHERE link.[AddressId] = address.[Id]);

                DELETE company
                FROM [Companies] AS company
                WHERE company.[Id] BETWEEN 1 AND 523
                  AND NOT EXISTS (
                      SELECT 1 FROM [PumpOperations] AS operation
                      WHERE operation.[CompanyId] = company.[Id])
                  AND NOT EXISTS (
                      SELECT 1 FROM [CompanyAddresses] AS link
                      WHERE link.[CompanyId] = company.[Id]);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_CompanyId_AddressId",
                table: "CompanyAddresses",
                columns: new[] { "CompanyId", "AddressId" },
                unique: true);

            // IX_Companies_Name already exists because migration v4 created it with raw SQL.
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Name",
                table: "Addresses",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyAddresses_CompanyId_AddressId",
                table: "CompanyAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_Name",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_CompanyId",
                table: "CompanyAddresses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Id",
                table: "Addresses",
                column: "Id",
                unique: true);

            // Seed rows are intentionally not restored. Administrators now own this data
            // through the Excel import flow.
        }
    }
}
