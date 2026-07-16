using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ProjectContext))]
    [Migration("20260716102000_FormatGreaterThanDistanceRange")]
    public partial class FormatGreaterThanDistanceRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE PumpOperations
                SET DistanceRange = N'> 100მ'
                WHERE DistanceRange = N'>100მ';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE PumpOperations
                SET DistanceRange = N'>100მ'
                WHERE DistanceRange = N'> 100მ';
                """);
        }
    }
}
