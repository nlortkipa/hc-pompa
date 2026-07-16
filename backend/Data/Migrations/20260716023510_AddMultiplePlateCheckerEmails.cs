using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiplePlateCheckerEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlateCheckerEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlateId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateCheckerEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateCheckerEmails_Plates_PlateId",
                        column: x => x.PlateId,
                        principalTable: "Plates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO PlateCheckerEmails (PlateId, Email)
                SELECT Id, LOWER(LTRIM(RTRIM(CheckerGmail)))
                FROM Plates
                WHERE LTRIM(RTRIM(CheckerGmail)) <> '';
                """);

            migrationBuilder.DropColumn(
                name: "CheckerGmail",
                table: "Plates");

            migrationBuilder.CreateIndex(
                name: "IX_PlateCheckerEmails_PlateId_Email",
                table: "PlateCheckerEmails",
                columns: new[] { "PlateId", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckerGmail",
                table: "Plates",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE Plates
                SET CheckerGmail = checker.Email
                FROM Plates
                OUTER APPLY (
                    SELECT TOP (1) Email
                    FROM PlateCheckerEmails
                    WHERE PlateCheckerEmails.PlateId = Plates.Id
                    ORDER BY PlateCheckerEmails.Id
                ) checker;
                """);

            migrationBuilder.DropTable(
                name: "PlateCheckerEmails");
        }
    }
}
