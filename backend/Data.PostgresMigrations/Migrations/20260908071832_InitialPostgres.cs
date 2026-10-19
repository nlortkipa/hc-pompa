using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonalId = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "User"),
                    Username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    TokenVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyAddresses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "PumpOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    OperatorId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    ConcreteVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    AdditionalStanding = table.Column<int>(type: "integer", nullable: false),
                    IsIdle = table.Column<bool>(type: "boolean", nullable: false),
                    DismantleAssemble = table.Column<int>(type: "integer", nullable: false),
                    PlateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PumpType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DistanceRange = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ReviewStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByCheckerGmail = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SmsCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsCodes_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PumpOperationReviewRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PumpOperationId = table.Column<int>(type: "integer", nullable: false),
                    CheckerGmail = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                });

            migrationBuilder.InsertData(
                table: "Plates",
                columns: new[] { "Id", "Number", "Type" },
                values: new object[,]
                {
                    { 1, "II210NI", "52მ" },
                    { 2, "PB107PB", "42მ" },
                    { 3, "BB685FF", "42მ" },
                    { 4, "VQ632QV", "42მ" },
                    { 5, "CC178LC", "<60მ" },
                    { 6, "WW832GW", "<60მ" },
                    { 7, "OV247VO", "47მ" },
                    { 8, "CG368CG", "48მ" },
                    { 9, "ᲰᲘᲓ. ᲒᲐᲛᲐᲜᲐᲬᲘᲚᲔᲑᲔᲚᲘ", "18მ" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "FirstName", "LastName", "PersonalId", "PhoneNumber", "Role", "Username" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "გაგა", "ლორთქიფანიძე", "11111111111", "577574401", "Admin", null },
                    { 2, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ჯემალ", "გიორგობიანი", "01001055244", "551710931", "User", null },
                    { 3, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "გიორგი", "გუჯეჯიანი", "01005027479", "598606823", "User", null },
                    { 4, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "დათო", "ელგენდარაშვილი", "01008045338", "577251148", "User", null },
                    { 5, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ზვიადი", "ენუქიძე", "38001031038", "591207381", "User", null },
                    { 6, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ლაშა", "ენუქიძე", "38001045932", "599061209", "User", null },
                    { 7, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "მამუკა", "კაპანაძე", "5400104439", "591662688", "User", null },
                    { 8, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "გელა", "მერებაშვილი", "24001038417", "595880560", "User", null },
                    { 9, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "გიორგი", "მილაძე", "24001043433", "551404068", "User", null },
                    { 10, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "მამუკა", "სამხარაძე", "38001011164", "597726630", "User", null },
                    { 11, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ნუგზარი", "სამხარაძე", "38001008077", "599868887", "User", null },
                    { 12, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ჯამბული", "ფავლენიშვილი", "18001063162", "555727171", "User", null },
                    { 13, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "გიორგი", "ფხაკაძე", "62001036043", "595417709", "User", null },
                    { 14, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ედიშერ", "ცხვიტაშვილი", "38001035765", "571410088", "User", null },
                    { 15, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ლევანი", "ცხვიტაშვილი", "38001013339", "571511331", "User", null },
                    { 16, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "დავითი", "ხუხია", "39001008605", "591504051", "User", null },
                    { 17, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ნოდარ", "ჯიქური", "24001046661", "551157052", "User", null },
                    { 18, null, new DateTime(2026, 6, 16, 12, 0, 0, 0, DateTimeKind.Utc), "ასიფ", "ვალიევი", "12001008021", "579904646", "User", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Name",
                table: "Addresses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_AddressId",
                table: "CompanyAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_CompanyId_AddressId",
                table: "CompanyAddresses",
                columns: new[] { "CompanyId", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlateCheckerEmails_PlateId_Email",
                table: "PlateCheckerEmails",
                columns: new[] { "PlateId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_PumpOperationId",
                table: "PumpOperationReviewRequests",
                column: "PumpOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpOperationReviewRequests_Token",
                table: "PumpOperationReviewRequests",
                column: "Token",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_SmsCodes_UserId",
                table: "SmsCodes",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_PersonalId",
                table: "users",
                column: "PersonalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_PhoneNumber",
                table: "users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyAddresses");

            migrationBuilder.DropTable(
                name: "PlateCheckerEmails");

            migrationBuilder.DropTable(
                name: "PumpOperationReviewRequests");

            migrationBuilder.DropTable(
                name: "SmsCodes");

            migrationBuilder.DropTable(
                name: "Plates");

            migrationBuilder.DropTable(
                name: "PumpOperations");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
