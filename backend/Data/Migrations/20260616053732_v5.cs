using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 25, 18 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 26, 19 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 27, 20 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 28, 21 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 29, 22 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 30,
                column: "AddressId",
                value: 30);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 31,
                column: "AddressId",
                value: 31);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 32, 23 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 33, 24 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 34, 25 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 24, 17 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 25, 18 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 26, 19 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 27, 20 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 28, 21 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 30,
                column: "AddressId",
                value: 29);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 31,
                column: "AddressId",
                value: 30);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 31, 22 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 32, 23 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 33, 24 });

            migrationBuilder.InsertData(
                table: "CompanyAddresses",
                columns: new[] { "Id", "AddressId", "CompanyId" },
                values: new object[] { 35, 34, 25 });
        }
    }
}
