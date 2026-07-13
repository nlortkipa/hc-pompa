using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Companies_Name'
                        AND [object_id] = OBJECT_ID(N'[dbo].[Companies]')
                )
                    DROP INDEX [IX_Companies_Name] ON [dbo].[Companies];
                """);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [dbo].[Companies] ON;

                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 6)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (6, N'__migration_company_6');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 8)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (8, N'__migration_company_8');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 10)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (10, N'__migration_company_10');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 13)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (13, N'__migration_company_13');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 14)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (14, N'__migration_company_14');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 22)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (22, N'__migration_company_22');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 24)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (24, N'__migration_company_24');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 25)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (25, N'__migration_company_25');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 31)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (31, N'__migration_company_31');
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [Id] = 32)
                    INSERT INTO [dbo].[Companies] ([Id], [Name]) VALUES (32, N'__migration_company_32');

                SET IDENTITY_INSERT [dbo].[Companies] OFF;
                """);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "რუსთავი, თბილისის ქუჩის 30-ის მიმდ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 26,
                column: "Name",
                value: "ბათუმი, ხახულის 63");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 27,
                column: "Name",
                value: "თბილისი, დიღომის მასივი, III კვარტალი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "რუსთავი, მოდებაძის ქუჩის მიმდებარედ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 29,
                column: "Name",
                value: "ბათუმი, ფრიდონ ხალვაშის 5 შესახვევის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "ხელვაჩაური, აკაკი შანიძის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 31,
                column: "Name",
                value: "ხელვაჩაური, ფრ. ხალვაშის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 32,
                column: "Name",
                value: "თბილისი, ენუქიძის 7");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "თბილისი, დ.ბაქრაძის ქ.20ბ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 34,
                column: "Name",
                value: "ბათუმი, ავ. შანიძის 7");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 35,
                column: "Name",
                value: "თბილისი, ლისი, ინტენსიფიკაციის ქუჩის მიმდ. (ნაკ N11/11)");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 36,
                column: "Name",
                value: "თბილისი, ელგუჯა ამაშუკელის 5");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 37,
                column: "Name",
                value: "თბილისი, ვარკეთილის მე-3 მასივი, მე8 კვ, კორპ17-ის მიმ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 38,
                column: "Name",
                value: "თბილისი, ხომერულის 27");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 39,
                column: "Name",
                value: "თბილისი, ვაჟა-ფშაველას II კვარტალი N9");

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

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 55,
                column: "Name",
                value: "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "შპს კ სამი თაუერ 405580198");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "შპს კ სამი ინვესტმენტი (405745234)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "შპს კავკასუს როუდ პროჯექტ (202258243)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "შპს კაზა კალდა დეველოფმენტი 404675026");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "შპს კაპიტალ ინვესტი (405307877)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11,
                column: "Name",
                value: "შპს კაპიტალ ქონსთრაქშენ საქართველო (406514829)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "შპს კერა + (205219351)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "შპს კერა 76 448426454");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 14,
                column: "Name",
                value: "შპს კერა ქონსთრაქშენი 400298316");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "შპს კომპანია ბლექ სი გრუპი (204477734)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "შპს კონსოლი 405059378");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "შპს კონსთრაქშენ გრუპი 445550216");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "შპს კონცეპტ ჰაუსი(405637244)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "შპს კორძან ელექტრონიქს 406221921");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "შპს ლაზერი+(402186228)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "შპს ლაზიკა 416332221");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "შპს ლაიფ ჰაუსი (445478563)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 23,
                column: "Name",
                value: "შპს ლეგაცი გრუპ (406325285)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 24,
                column: "Name",
                value: "შპს ლევილი პირველი 445595598");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "შპს ლიდერ ბეტონი (248384449)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 26,
                column: "Name",
                value: "შპს ლისი არეა (405525668)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 27,
                column: "Name",
                value: "შპს ლისი თაუერსი 405535915");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "შპს ლიტ ჯეო ინვესტ 243859749");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 29,
                column: "Name",
                value: "შპს ლივიორი (401968082)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "შპს მ.გ.ფ.გ. (406099270)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 31,
                column: "Name",
                value: "შპს მალუ კვადრატი 417899180");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 32,
                column: "Name",
                value: "შპს მარნეული რუსთავი 405391428");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "შპს მაურო დეველოფმენტი 405682603");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 34,
                column: "Name",
                value: "შპს მაურო ქონსთრაქშენ (445433120)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 35,
                column: "Name",
                value: "შპს მბ აგრო 436035559");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 36,
                column: "Name",
                value: "შპს მგს-ს საქართველოს ფილიალი (205168539)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 37,
                column: "Name",
                value: "შპს მეგა ცენტრი (402192943)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 38,
                column: "Name",
                value: "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 39,
                column: "Name",
                value: "შპს მეტონტაჟი 2014 (415091457)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 40,
                column: "Name",
                value: "შპს მეონ ინტერნეიშენალ 208213084");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "შპს მურაჟი მოუნთი (405625015)");

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 6,
                column: "CompanyId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 7,
                column: "CompanyId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 8,
                column: "CompanyId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 9,
                column: "CompanyId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 10,
                column: "CompanyId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 11,
                column: "CompanyId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 12,
                column: "CompanyId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 13,
                column: "CompanyId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 14,
                column: "CompanyId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 15,
                column: "CompanyId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 16,
                column: "CompanyId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 17,
                column: "CompanyId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 18,
                column: "CompanyId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 19,
                column: "CompanyId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 20,
                column: "CompanyId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 21,
                column: "CompanyId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 22,
                column: "CompanyId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 23,
                column: "CompanyId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 24,
                column: "CompanyId",
                value: 17);

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
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 29, 22 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 30, 22 });

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

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 34, 25 });

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
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 40, 31 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 41, 32 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 42, 33 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 43, 34 });

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
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 44, 35 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 45, 36 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 46, 37 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 47, 38 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 48, 38 });

            migrationBuilder.InsertData(
                table: "CompanyAddresses",
                columns: new[] { "Id", "AddressId", "CompanyId" },
                values: new object[,]
                {
                    { 51, 49, 38 },
                    { 52, 50, 38 },
                    { 53, 51, 39 },
                    { 54, 52, 39 },
                    { 55, 53, 40 },
                    { 56, 54, 41 },
                    { 57, 55, 41 }
                });

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Companies_Name'
                        AND [object_id] = OBJECT_ID(N'[dbo].[Companies]')
                )
                    CREATE UNIQUE INDEX [IX_Companies_Name] ON [dbo].[Companies] ([Name]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "ბათუმი, ტბელ აბუსერიძის 19");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 26,
                column: "Name",
                value: "რუსთავი, თბილისის ქუჩის 30-ის მიმდ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 27,
                column: "Name",
                value: "ბათუმი, ხახულის 63");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "თბილისი, დიღომის მასივი, III კვარტალი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 29,
                column: "Name",
                value: "რუსთავი, მოდებაძის ქუჩის მიმდებარედ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "ბათუმი, ფრიდონ ხალვაშის 5 შესახვევის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 31,
                column: "Name",
                value: "ხელვაჩაური, აკაკი შანიძის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 32,
                column: "Name",
                value: "ხელვაჩაური, ფრ. ხალვაშის 13");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "თბილისი, ენუქიძის 7");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 34,
                column: "Name",
                value: "თბილისი, დ.ბაქრაძის ქ.20ბ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 35,
                column: "Name",
                value: "ბათუმი, ავ. შანიძის 7");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 36,
                column: "Name",
                value: "თბილისი, ლისი, ინტენსიფიკაციის ქუჩის მიმდ. (ნაკ N11/11)");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 37,
                column: "Name",
                value: "თბილისი, ელგუჯა ამაშუკელის 5");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 38,
                column: "Name",
                value: "თბილისი, ვარკეთილის მე-3 მასივი, მე8 კვ, კორპ17-ის მიმ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 39,
                column: "Name",
                value: "თბილისი, ხომერულის 27");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 40,
                column: "Name",
                value: "თბილისი, ვაჟა-ფშაველას II კვარტალი N9");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "გურჯაანი, სოფელი არაშენდა");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 42,
                column: "Name",
                value: "რუსთავი, ნიკო ლომოურის ქ.9გ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 43,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ 30");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 44,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "თბილისი, ნოე რამიშვილის ქ 30");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 46,
                column: "Name",
                value: "თბილისი, თვალჭრელიძის 35ა");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 47,
                column: "Name",
                value: "გარდაბანი, ჯანდარა, CSG 1 BP პროექტი");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "გარდაბანი, დავით აღმაშენებლის ქ. 34");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 49,
                column: "Name",
                value: "თბილისი, ქიზიყის 28");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "თბილისი, ნიზამიშვილის 262ბ");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 51,
                column: "Name",
                value: "რუსთავი ,ახმეტელის ქ#8 (25-ე საბავშვო ბაღთან)");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 52,
                column: "Name",
                value: "საგარეჯო, სოფელი ლამბალო");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 53,
                column: "Name",
                value: "ფოთი, დანელიას ჟ#84");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 54,
                column: "Name",
                value: "ფოთი, წმინდა გიორგის ქ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 55,
                column: "Name",
                value: "თბილისი, ქავთარაძის 20");

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 56, "გურჯაანი, სოფ.კაჭრეთი" },
                    { 57, "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ" }
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "შპს კ სამი 405382955");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "შპს კ სამი თაუერ 405580198");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "შპს კ სამი თაუერ 405580198");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "შპს კ სამი ინვესტმენტი (405745234)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "შპს კ სამი ინვესტმენტი (405745234)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11,
                column: "Name",
                value: "შპს კავკასუს როუდ პროჯექტ (202258243)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "შპს კაზა კალდა დეველოფმენტი 404675026");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "შპს კაზა კალდა დეველოფმენტი 404675026");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 14,
                column: "Name",
                value: "შპს კაზა კალდა დეველოფმენტი 404675026");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "შპს კაპიტალ ინვესტი (405307877)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "შპს კაპიტალ ქონსთრაქშენ საქართველო (406514829)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "შპს კერა + (205219351)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "შპს კერა 76 448426454");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "შპს კერა ქონსთრაქშენი 400298316");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "შპს კომპანია ბლექ სი გრუპი (204477734)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "შპს კონსოლი 405059378");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "შპს კონსოლი 405059378");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 23,
                column: "Name",
                value: "შპს კონსთრაქშენ გრუპი 445550216");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 24,
                column: "Name",
                value: "შპს კონსთრაქშენ გრუპი 445550216");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "შპს კონსთრაქშენ გრუპი 445550216");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 26,
                column: "Name",
                value: "შპს კონცეპტ ჰაუსი(405637244)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 27,
                column: "Name",
                value: "შპს კორძან ელექტრონიქს 406221921");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "შპს ლაზერი+(402186228)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 29,
                column: "Name",
                value: "შპს ლაზიკა 416332221");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "შპს ლაიფ ჰაუსი (445478563)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 31,
                column: "Name",
                value: "შპს ლაიფ ჰაუსი (445478563)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 32,
                column: "Name",
                value: "შპს ლაიფ ჰაუსი (445478563)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "შპს ლეგაცი გრუპ (406325285)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 34,
                column: "Name",
                value: "შპს ლევილი პირველი 445595598");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 35,
                column: "Name",
                value: "შპს ლიდერ ბეტონი (248384449)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 36,
                column: "Name",
                value: "შპს ლისი არეა (405525668)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 37,
                column: "Name",
                value: "შპს ლისი თაუერსი 405535915");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 38,
                column: "Name",
                value: "შპს ლიტ ჯეო ინვესტ 243859749");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 39,
                column: "Name",
                value: "შპს ლივიორი (401968082)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 40,
                column: "Name",
                value: "შპს მ.გ.ფ.გ. (406099270)");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "შპს მალუ კვადრატი 417899180");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 42, "შპს მარნეული რუსთავი 405391428" },
                    { 43, "შპს მაურო დეველოფმენტი 405682603" },
                    { 44, "შპს მაურო ქონსთრაქშენ (445433120)" },
                    { 45, "შპს მაურო ქონსთრაქშენ (445433120)" },
                    { 46, "შპს მბ აგრო 436035559" },
                    { 47, "შპს მგს-ს საქართველოს ფილიალი (205168539)" },
                    { 48, "შპს მეგა ცენტრი (402192943)" },
                    { 49, "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)" },
                    { 50, "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)" },
                    { 51, "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)" },
                    { 52, "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)" },
                    { 53, "შპს მეტონტაჟი 2014 (415091457)" },
                    { 54, "შპს მეტონტაჟი 2014 (415091457)" },
                    { 55, "შპს მეონ ინტერნეიშენალ 208213084" },
                    { 56, "შპს მურაჟი მოუნთი (405625015)" },
                    { 57, "შპს მურაჟი მოუნთი (405625015)" }
                });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 6,
                column: "CompanyId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 7,
                column: "CompanyId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 8,
                column: "CompanyId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 9,
                column: "CompanyId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 10,
                column: "CompanyId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 11,
                column: "CompanyId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 12,
                column: "CompanyId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 13,
                column: "CompanyId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 14,
                column: "CompanyId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 15,
                column: "CompanyId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 16,
                column: "CompanyId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 17,
                column: "CompanyId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 18,
                column: "CompanyId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 19,
                column: "CompanyId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 20,
                column: "CompanyId",
                value: 20);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 21,
                column: "CompanyId",
                value: 21);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 22,
                column: "CompanyId",
                value: 22);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 23,
                column: "CompanyId",
                value: 23);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 24,
                column: "CompanyId",
                value: 24);

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 25, 25 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 26, 26 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 27, 27 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 28, 28 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 29, 29 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 30, 30 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 31, 31 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 32, 32 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 33, 33 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 34, 34 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 35, 35 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 36, 36 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 37, 37 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 38, 38 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 39, 39 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 40, 40 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 41, 41 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 42, 42 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 43, 43 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 44, 44 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 45, 45 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 46, 46 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 47, 47 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 48, 48 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 49, 49 });

            migrationBuilder.UpdateData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "AddressId", "CompanyId" },
                values: new object[] { 50, 50 });
        }
    }
}
