using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPumpOperationReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewStatus",
                table: "PumpOperations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.Sql("UPDATE [PumpOperations] SET [ReviewStatus] = 'Accepted' WHERE [ReviewStatus] IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewStatus",
                table: "PumpOperations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "PumpOperations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByReviewerId",
                table: "PumpOperations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PumpOperationReviewRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PumpOperationId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpOperationReviewRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumpOperationReviewRequests_PumpOperations_PumpOperationId",
                        column: x => x.PumpOperationId,
                        principalTable: "PumpOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpOperationReviewRequests_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperations_ReviewedByReviewerId",
                table: "PumpOperations",
                column: "ReviewedByReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_PumpOperationId",
                table: "PumpOperationReviewRequests",
                column: "PumpOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_ReviewerId",
                table: "PumpOperationReviewRequests",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_Token",
                table: "PumpOperationReviewRequests",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_Gmail",
                table: "Reviewers",
                column: "Gmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_PhoneNumber",
                table: "Reviewers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations",
                column: "ReviewedByReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropTable(
                name: "PumpOperationReviewRequests");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_PumpOperations_ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewedByReviewerId",
                table: "PumpOperations");
        }
    }
}
