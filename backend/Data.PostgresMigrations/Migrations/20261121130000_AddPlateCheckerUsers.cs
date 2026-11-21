using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPlateCheckerUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlateCheckerEmails");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "users",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckerGmail",
                table: "PumpOperationReviewRequests",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<int>(
                name: "CheckerUserId",
                table: "PumpOperationReviewRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlateCheckers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlateId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateCheckers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlateCheckers_Plates_PlateId",
                        column: x => x.PlateId,
                        principalTable: "Plates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlateCheckers_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_CheckerUserId",
                table: "PumpOperationReviewRequests",
                column: "CheckerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlateCheckers_PlateId_UserId",
                table: "PlateCheckers",
                columns: new[] { "PlateId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlateCheckers_UserId",
                table: "PlateCheckers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PumpOperationReviewRequests_users_CheckerUserId",
                table: "PumpOperationReviewRequests",
                column: "CheckerUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PumpOperationReviewRequests_users_CheckerUserId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropTable(
                name: "PlateCheckers");

            migrationBuilder.DropIndex(
                name: "IX_PumpOperationReviewRequests_CheckerUserId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CheckerUserId",
                table: "PumpOperationReviewRequests");

            migrationBuilder.AlterColumn<string>(
                name: "CheckerGmail",
                table: "PumpOperationReviewRequests",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PlateCheckerEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlateId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_PlateCheckerEmails_PlateId_Email",
                table: "PlateCheckerEmails",
                columns: new[] { "PlateId", "Email" },
                unique: true);
        }
    }
}
