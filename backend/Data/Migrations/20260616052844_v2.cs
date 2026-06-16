using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "თბილისი, ბაგები წყნეთის გზატკეცილი 40");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "თბილისი, გაზაფხულის ქუჩა 79");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "თბილისი, ევგენია მაისურაძის ქ 7");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "რუსთავი, XVI მკრ.");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "თბილისი,ვაშლიჯვარი. მანდორ პეტეფის 54");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "სოფელი დიღომი, ფატმანის 7");

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 7, "თბილისი, გურამიშვილის 64" },
                    { 8, "თბილისი, ციხისძირის 69 (ნავთლუღი)" },
                    { 9, "თბილისი, დიღომი, როსტევანის 1 ჩიხი N3" },
                    { 10, "თბილისი, სოფელი დიღომი, ფატმანის 5" },
                    { 11, "მცხეთა, ნატახტარი" },
                    { 12, "ბათუმი, გრიგოლ ელიავას 41 ა" },
                    { 13, "თბილისი,ვახტანგ ჩიქოვანის N2" },
                    { 14, "თბილისი,პოლიციის #7" },
                    { 15, "თბილისი,ზურაბ ანჯაფარიძის 34" },
                    { 16, "თბილისი, ზ.ფალიაშვილის 100" },
                    { 17, "ახმეტის რაიონი,სოფ. ალავერდი" },
                    { 18, "ბათუმი, აეროპორტის გზატკეცილი 243" },
                    { 19, "თბილისი, ცოტნე დადიანის 263" },
                    { 20, "გურჯაანი, სოფ არაშენდა" },
                    { 21, "თბილისი, ცაცხვების 13;15;17" },
                    { 22, "თბილისი, ცაცხვების ქუჩა 15-17; მე-9 შესახვევი" },
                    { 23, "ბათუმი, ტბელ აბუსერიძის 23" },
                    { 24, "ბათუმი, ტბელ აბუსერიძის 19" },
                    { 25, "ბათუმი, ტბელ აბუსერიძის 19" },
                    { 26, "რუსთავი, თბილისის ქუჩის 30-ის მიმდ." },
                    { 27, "ბათუმი, ხახულის 63" },
                    { 28, "თბილისი, დიღომის მასივი, III კვარტალი" },
                    { 29, "რუსთავი, მოდებაძის ქუჩის მიმდებარედ" },
                    { 30, "ბათუმი, ფრიდონ ხალვაშის 5 შესახვევის 13" },
                    { 31, "ხელვაჩაური, აკაკი შანიძის 13" },
                    { 32, "ხელვაჩაური, ფრ. ხალვაშის 13" },
                    { 33, "თბილისი, ენუქიძის 7" },
                    { 34, "თბილისი, დ.ბაქრაძის ქ.20ბ" },
                    { 35, "ბათუმი, ავ. შანიძის 7" },
                    { 36, "თბილისი, ლისი, ინტენსიფიკაციის ქუჩის მიმდ. (ნაკ N11/11)" },
                    { 37, "თბილისი, ელგუჯა ამაშუკელის 5" },
                    { 38, "თბილისი, ვარკეთილის მე-3 მასივი, მე8 კვ, კორპ17-ის მიმ" },
                    { 39, "თბილისი, ხომერულის 27" },
                    { 40, "თბილისი, ვაჟა-ფშაველას II კვარტალი N9" },
                    { 41, "გურჯაანი, სოფელი არაშენდა" },
                    { 42, "რუსთავი, ნიკო ლომოურის ქ.9გ" },
                    { 43, "თბილისი, ნოე რამიშვილის ქ 30" },
                    { 44, "თბილისი, ნოე რამიშვილის ქ" },
                    { 45, "თბილისი, ნოე რამიშვილის ქ 30" },
                    { 46, "თბილისი, თვალჭრელიძის 35ა" },
                    { 47, "გარდაბანი, ჯანდარა, CSG 1 BP პროექტი" },
                    { 48, "გარდაბანი, დავით აღმაშენებლის ქ. 34" },
                    { 49, "თბილისი, ქიზიყის 28" },
                    { 50, "თბილისი, ნიზამიშვილის 262ბ" },
                    { 51, "რუსთავი ,ახმეტელის ქ#8 (25-ე საბავშვო ბაღთან)" },
                    { 52, "საგარეჯო, სოფელი ლამბალო" },
                    { 53, "ფოთი, დანელიას ჟ#84" },
                    { 54, "ფოთი, წმინდა გიორგის ქ." },
                    { 55, "თბილისი, ქავთარაძის 20" },
                    { 56, "გურჯაანი, სოფ.კაჭრეთი" },
                    { 57, "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "შპს ისთვისის მენეჯმენტ კომპანი 406374490" },
                    { 5, "შპს კ სამი 405382955" },
                    { 7, "შპს კ სამი თაუერ 405580198" },
                    { 9, "შპს კ სამი ინვესტმენტი (405745234)" },
                    { 11, "შპს კავკასუს როუდ პროჯექტ (202258243)" },
                    { 12, "შპს კაზა კალდა დეველოფმენტი 404675026 - T" },
                    { 15, "შპს კაპიტალ ინვესტი (405307877)" },
                    { 16, "შპს კაპიტალ ქონსთრაქშენ საქართველო (406514829)" },
                    { 17, "შპს კერა + (205219351)" },
                    { 18, "შპს კერა 76 448426454" },
                    { 19, "შპს კერა ქონსთრაქშენი 400298316" },
                    { 20, "შპს კომპანია ბლექ სი გრუპი (204477734)" },
                    { 21, "შპს კონსოლი 405059378" },
                    { 23, "შპს კონსთრაქშენ გრუპი 445550216 - T" },
                    { 26, "შპს კონცეპტ ჰაუსი(405637244)" },
                    { 27, "შპს კორძან ელექტრონიქს 406221921" },
                    { 28, "შპს ლაზერი+(402186228)" },
                    { 29, "შპს ლაზიკა 416332221" },
                    { 30, "შპს ლაიფ ჰაუსი (445478563) - T" },
                    { 33, "შპს ლეგაცი გრუპ (406325285)" },
                    { 34, "შპს ლევილი პირველი 445595598" },
                    { 35, "შპს ლიდერ ბეტონი (248384449)" },
                    { 36, "შპს ლისი არეა (405525668)" },
                    { 37, "შპს ლისი თაუერსი 405535915" },
                    { 38, "შპს ლიტ ჯეო ინვესტ 243859749" },
                    { 39, "შპს ლივიორი (401968082)" },
                    { 40, "შპს მ.გ.ფ.გ. (406099270)" },
                    { 41, "შპს მალუ კვადრატი 417899180" },
                    { 42, "შპს მარნეული რუსთავი 405391428" },
                    { 43, "შპს მაურო დეველოფმენტი 405682603" },
                    { 44, "შპს მაურო ქონსთრაქშენ (445433120)" },
                    { 46, "შპს მბ აგრო 436035559" },
                    { 47, "შპს მგს-ს საქართველოს ფილიალი (205168539)" },
                    { 48, "შპს მეგა ცენტრი (402192943)" },
                    { 49, "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი) - T" },
                    { 53, "შპს მეტონტაჟი 2014 (415091457)" },
                    { 55, "შპს მეონ ინტერნეიშენალ 208213084" },
                    { 56, "შპს მურაჟი მოუნთი (405625015)" }
                });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "FirstName", "PhoneNumber" },
                values: new object[] { new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "გაგა", "577574401" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "FirstName", "LastName", "PersonalId", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ჯემალ", "გიორგობიანი", "1001055244", "551710931", "User" },
                    { 3, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "გიორგი", "გუჯეჯიანი", "1005027479", "598606823", "User" },
                    { 4, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "დათო", "ელგენდარაშვილი", "1008045338", "577251148", "User" },
                    { 5, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ზვიადი", "ენუქიძე", "38001031038", "591207381", "User" },
                    { 6, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ლაშა", "ენუქიძე", "38001045932", "599061209", "User" },
                    { 7, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "მამუკა", "კაპანაძე", "5400104439", "591662688", "User" },
                    { 8, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "გელა", "მერებაშვილი", "24001038417", "595880560", "User" },
                    { 9, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "გიორგი", "მილაძე", "24001043433", "551404068", "User" },
                    { 10, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "მამუკა", "სამხარაძე", "38001011164", "597726630", "User" },
                    { 11, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ნუგზარი", "სამხარაძე", "38001008077", "599868887", "User" },
                    { 12, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ჯამბული", "ფავლენიშვილი", "18001063162", "555727171", "User" },
                    { 13, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "გიორგი", "ფხაკაძე", "62001036043", "595417709", "User" },
                    { 14, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ედიშერ", "ცხვიტაშვილი", "38001035765", "571410088", "User" },
                    { 15, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ლევანი", "ცხვიტაშვილი", "38001013339", "571511331", "User" },
                    { 16, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "დავითი", "ხუხია", "39001008605", "591504051", "User" },
                    { 17, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ნოდარ", "ჯიქური", "24001046661", "551157052", "User" },
                    { 18, new DateTime(2026, 6, 16, 16, 0, 0, 0, DateTimeKind.Local), "ასიფ", "ვალიევი", "12001008021", "579904646", "User" }
                });

            migrationBuilder.InsertData(
                table: "CompanyAddresses",
                columns: new[] { "Id", "AddressId", "CompanyId" },
                values: new object[,]
                {
                    { 7, 7, 7 },
                    { 9, 9, 9 },
                    { 11, 11, 11 },
                    { 12, 12, 12 },
                    { 15, 15, 15 },
                    { 16, 16, 16 },
                    { 17, 17, 17 },
                    { 18, 18, 18 },
                    { 19, 19, 19 },
                    { 20, 20, 20 },
                    { 21, 21, 21 },
                    { 23, 23, 23 },
                    { 26, 26, 26 },
                    { 27, 27, 27 },
                    { 28, 28, 28 },
                    { 29, 29, 29 },
                    { 30, 30, 30 },
                    { 33, 33, 33 },
                    { 34, 34, 34 },
                    { 35, 35, 35 },
                    { 36, 36, 36 },
                    { 37, 37, 37 },
                    { 38, 38, 38 },
                    { 39, 39, 39 },
                    { 40, 40, 40 },
                    { 41, 41, 41 },
                    { 42, 42, 42 },
                    { 43, 43, 43 },
                    { 44, 44, 44 },
                    { 46, 46, 46 },
                    { 47, 47, 47 },
                    { 48, 48, 48 },
                    { 49, 49, 49 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyAddresses",
                keyColumn: "Id",
                keyValues: new object[] { 7, 9, 11, 12, 15, 16, 17, 18, 19, 20, 21, 23, 26, 27, 28, 29, 30, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49 });

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValues: new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 });

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValues: new object[] { 4, 5, 7, 9, 11, 12, 15, 16, 17, 18, 19, 20, 21, 23, 26, 27, 28, 29, 30, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49, 53, 55, 56 });

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValues: new object[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 });

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Address 1");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Address 2");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Address 3");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Address 4");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Address 5");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Address 6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "FirstName", "PhoneNumber" },
                values: new object[] { new DateTime(2026, 5, 28, 16, 0, 0, 0, DateTimeKind.Local), "ნიკოლოზ", "575780323" });
        }
    }
}
