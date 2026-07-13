using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RepairMissingCompanyAddressSeedLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SET IDENTITY_INSERT [CompanyAddresses] ON;

INSERT INTO [CompanyAddresses] ([Id], [CompanyId], [AddressId])
SELECT v.[Id], v.[CompanyId], v.[AddressId]
FROM (VALUES
    (8, 5, 8),
    (10, 7, 10),
    (13, 10, 13),
    (14, 11, 14),
    (22, 19, 21),
    (24, 21, 23),
    (25, 22, 24),
    (31, 28, 30),
    (32, 29, 31),
    (42, 39, 41),
    (43, 40, 42),
    (44, 41, 43),
    (45, 42, 44),
    (46, 43, 45),
    (47, 44, 46),
    (48, 45, 47),
    (49, 46, 48),
    (50, 47, 49)
) AS v([Id], [CompanyId], [AddressId])
WHERE NOT EXISTS (
    SELECT 1
    FROM [CompanyAddresses] ca
    WHERE ca.[Id] = v.[Id]
);

SET IDENTITY_INSERT [CompanyAddresses] OFF;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: these seed links may already exist before this idempotent repair runs.
        }
    }
}
