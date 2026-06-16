using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 40,
                column: "Name",
                value: "გურჯაანი, ნიკო ლომოურის ქ.9გ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ 30");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 42,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 43,
                column: "Name",
                value: "თბილისი, თვალჭრელიძის 35ა");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 44,
                column: "Name",
                value: "გარდაბანი, ჯანდარა, CSG 1 BP პროექტი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "გარდაბანი, დავით აღმაშენებლის ქ. 34");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 46,
                column: "Name",
                value: "თბილისი, ქიზიყის 28");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 47,
                column: "Name",
                value: "თბილისი, ნიზამიშვილის 262ბ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "რუსთავი ,ახმეტელის ქ#8 (25-ე საბავშვო ბაღთან)");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 49,
                column: "Name",
                value: "საგარეჯო, სოფელი ლამბალო");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "ფოთი, დანელიას ჟ#84");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 51,
                column: "Name",
                value: "ფოთი, წმინდა გიორგის ქ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 52,
                column: "Name",
                value: "თბილისი, ქავთარაძის 20");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 53,
                column: "Name",
                value: "გურჯაანი, სოფ.კაჭრეთი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 54,
                column: "Name",
                value: "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ");

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 36, 27 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 37, 28 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 38, 29 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 39, 30 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 20, 31 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 41,
                column: "CompanyId",
                value: 32);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 42,
                column: "CompanyId",
                value: 33);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 43,
                column: "CompanyId",
                value: 34);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 44,
                column: "AddressId",
                value: 41);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 43, 35 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 46,
                column: "CompanyId",
                value: 36);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 47,
                column: "CompanyId",
                value: 37);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 48,
                column: "CompanyId",
                value: 38);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 52,
                column: "CompanyId",
                value: 39);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 54,
                column: "CompanyId",
                value: 40);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 55,
                column: "CompanyId",
                value: 41);

            migrationBuilder.InsertData(
                table: "CompanyAddresses",
                columns: new[] { "Id", "AddressId", "CompanyId" },
                values: new object[] { 35, 35, 26 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 40,
                column: "Name",
                value: "გურჯაანი, სოფელი არაშენდა");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "რუსთავი, ნიკო ლომოურის ქ.9გ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 42,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ 30");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 43,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 44,
                column: "Name",
                value: "თბილისი, თვალჭრელიძის 35ა");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "გარდაბანი, ჯანდარა, CSG 1 BP პროექტი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 46,
                column: "Name",
                value: "გარდაბანი, დავით აღმაშენებლის ქ. 34");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 47,
                column: "Name",
                value: "თბილისი, ქიზიყის 28");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "თბილისი, ნიზამიშვილის 262ბ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 49,
                column: "Name",
                value: "რუსთავი ,ახმეტელის ქ#8 (25-ე საბავშვო ბაღთან)");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "საგარეჯო, სოფელი ლამბალო");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 51,
                column: "Name",
                value: "ფოთი, დანელიას ჟ#84");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 52,
                column: "Name",
                value: "ფოთი, წმინდა გიორგის ქ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 53,
                column: "Name",
                value: "თბილისი, ქავთარაძის 20");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 54,
                column: "Name",
                value: "გურჯაანი, სოფ.კაჭრეთი");

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 55, "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ" });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 35, 26 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 36, 27 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 37, 28 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 38, 29 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 39, 30 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 41,
                column: "CompanyId",
                value: 31);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 42,
                column: "CompanyId",
                value: 32);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 43,
                column: "CompanyId",
                value: 33);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 44,
                column: "AddressId",
                value: 43);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 42, 34 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 46,
                column: "CompanyId",
                value: 35);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 47,
                column: "CompanyId",
                value: 36);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 48,
                column: "CompanyId",
                value: 37);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 52,
                column: "CompanyId",
                value: 38);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 54,
                column: "CompanyId",
                value: 39);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 55,
                column: "CompanyId",
                value: 40);

            migrationBuilder.InsertData(
                table: "CompanyAddresses",
                columns: new[] { "Id", "AddressId", "CompanyId" },
                values: new object[] { 57, 55, 41 });
        }
    }
}
