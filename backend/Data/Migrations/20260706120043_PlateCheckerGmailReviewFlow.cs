using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class PlateCheckerGmailReviewFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperationReviewRequests_Reviewers_ReviewerId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_PumpOperations_ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropIndex(
                name: "IX_PumpOperationReviewRequests_ReviewerId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewedByReviewerName",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "ReviewerPhoneNumber",
                table: "PumpOperationReviewRequests");

            migrationBuilder.RenameColumn(
                name: "ReviewedByReviewerGmail",
                table: "PumpOperations",
                newName: "ReviewedByCheckerGmail");

            migrationBuilder.RenameColumn(
                name: "ReviewerGmail",
                table: "PumpOperationReviewRequests",
                newName: "CheckerGmail");

            migrationBuilder.AddColumn<string>(
                name: "CheckerGmail",
                table: "Plates",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 5,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 6,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.UpdateData(
                table: "Plates",
                keyColumn: "Id",
                keyValue: 7,
                column: "CheckerGmail",
                value: "");

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.CheckerGmail = latest.CheckerGmail
                FROM Plates p
                CROSS APPLY (
                    SELECT TOP 1 rr.CheckerGmail
                    FROM PumpOperations po
                    INNER JOIN PumpOperationReviewRequests rr ON rr.PumpOperationId = po.Id
                    WHERE po.PlateNumber = p.Number AND rr.CheckerGmail <> ''
                    ORDER BY rr.CreatedAt DESC
                ) latest
                WHERE p.CheckerGmail = ''
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckerGmail",
                table: "Plates");

            migrationBuilder.RenameColumn(
                name: "ReviewedByCheckerGmail",
                table: "PumpOperations",
                newName: "ReviewedByReviewerGmail");

            migrationBuilder.RenameColumn(
                name: "CheckerGmail",
                table: "PumpOperationReviewRequests",
                newName: "ReviewerGmail");

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByReviewerId",
                table: "PumpOperations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByReviewerName",
                table: "PumpOperations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewerId",
                table: "PumpOperationReviewRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "PumpOperationReviewRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerPhoneNumber",
                table: "PumpOperationReviewRequests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperations_ReviewedByReviewerId",
                table: "PumpOperations",
                column: "ReviewedByReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_ReviewerId",
                table: "PumpOperationReviewRequests",
                column: "ReviewerId");

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
                name: "FK_PumpOperationReviewRequests_Reviewers_ReviewerId",
                table: "PumpOperationReviewRequests",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations",
                column: "ReviewedByReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
