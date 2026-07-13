using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UseHardDeleteForReviewers : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByReviewerGmail",
                table: "PumpOperations",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByReviewerName",
                table: "PumpOperations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReviewerId",
                table: "PumpOperationReviewRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerGmail",
                table: "PumpOperationReviewRequests",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.Sql(@"
                UPDATE rr
                SET
                    rr.ReviewerName = r.Name,
                    rr.ReviewerGmail = r.Gmail,
                    rr.ReviewerPhoneNumber = r.PhoneNumber
                FROM PumpOperationReviewRequests rr
                INNER JOIN Reviewers r ON rr.ReviewerId = r.Id
                WHERE rr.ReviewerName = '' OR rr.ReviewerGmail = '' OR rr.ReviewerPhoneNumber = ''
            ");

            migrationBuilder.Sql(@"
                UPDATE po
                SET
                    po.ReviewedByReviewerName = r.Name,
                    po.ReviewedByReviewerGmail = r.Gmail
                FROM PumpOperations po
                INNER JOIN Reviewers r ON po.ReviewedByReviewerId = r.Id
                WHERE po.ReviewedByReviewerId IS NOT NULL
            ");

            migrationBuilder.Sql(@"
                UPDATE PumpOperationReviewRequests
                SET ReviewerId = NULL
                WHERE ReviewerId IN (SELECT Id FROM Reviewers WHERE IsActive = 0)
            ");

            migrationBuilder.Sql(@"
                UPDATE PumpOperations
                SET ReviewedByReviewerId = NULL
                WHERE ReviewedByReviewerId IN (SELECT Id FROM Reviewers WHERE IsActive = 0)
            ");

            migrationBuilder.Sql("DELETE FROM Reviewers WHERE IsActive = 0");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperationReviewRequests_Reviewers_ReviewerId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewedByReviewerGmail",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewedByReviewerName",
                table: "PumpOperations");

            migrationBuilder.DropColumn(
                name: "ReviewerGmail",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "ReviewerPhoneNumber",
                table: "PumpOperationReviewRequests");

            migrationBuilder.Sql("DELETE FROM PumpOperationReviewRequests WHERE ReviewerId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewerId",
                table: "PumpOperationReviewRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PumpOperationReviewRequests_Reviewers_ReviewerId",
                table: "PumpOperationReviewRequests",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PumpOperations_Reviewers_ReviewedByReviewerId",
                table: "PumpOperations",
                column: "ReviewedByReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
