using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ProjectContext))]
    [Migration("20260716101000_NormalizeDistanceRangeUnits")]
    public partial class NormalizeDistanceRangeUnits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE PumpOperations
                SET DistanceRange = CASE DistanceRange
                    WHEN '< 100' THEN N'<100მ'
                    WHEN '> 100' THEN N'>100მ'
                    ELSE DistanceRange
                END
                WHERE DistanceRange IN ('< 100', '> 100');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE PumpOperations
                SET DistanceRange = CASE DistanceRange
                    WHEN N'<100მ' THEN '< 100'
                    WHEN N'>100მ' THEN '> 100'
                    ELSE DistanceRange
                END
                WHERE DistanceRange IN (N'<100მ', N'>100მ');
                """);
        }
    }
}
