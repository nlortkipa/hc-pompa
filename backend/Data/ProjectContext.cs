using Data.Configurations;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data
{
    public class ProjectContext : DbContext
    {
        // DBSet Properties
        public DbSet<User> Users { get; set; }
        public DbSet<SmsCode> SmsCodes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CompanyAddress> CompanyAddresses { get; set; }
        public DbSet<PumpOperation> PumpOperations { get; set; }

        public ProjectContext()
        {
            
        }

        public ProjectContext(DbContextOptions<ProjectContext> context) : base(context)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // add configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SmsCodeConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyAddressConfiguration());
            modelBuilder.ApplyConfiguration(new PumpOperationConfiguration());

            // seeding
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "გაგა",
                    LastName = "ლორთქიფანიძე",
                    PersonalId = "11111111111",
                    PhoneNumber = "577574401",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    FirstName = "ჯემალ",
                    LastName = "გიორგობიანი",
                    PersonalId = "1001055244",
                    PhoneNumber = "551710931",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 3,
                    FirstName = "გიორგი",
                    LastName = "გუჯეჯიანი",
                    PersonalId = "1005027479",
                    PhoneNumber = "598606823",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 4,
                    FirstName = "დათო",
                    LastName = "ელგენდარაშვილი",
                    PersonalId = "1008045338",
                    PhoneNumber = "577251148",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 5,
                    FirstName = "ზვიადი",
                    LastName = "ენუქიძე",
                    PersonalId = "38001031038",
                    PhoneNumber = "591207381",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 6,
                    FirstName = "ლაშა",
                    LastName = "ენუქიძე",
                    PersonalId = "38001045932",
                    PhoneNumber = "599061209",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 7,
                    FirstName = "მამუკა",
                    LastName = "კაპანაძე",
                    PersonalId = "5400104439",
                    PhoneNumber = "591662688",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 8,
                    FirstName = "გელა",
                    LastName = "მერებაშვილი",
                    PersonalId = "24001038417",
                    PhoneNumber = "595880560",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 9,
                    FirstName = "გიორგი",
                    LastName = "მილაძე",
                    PersonalId = "24001043433",
                    PhoneNumber = "551404068",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 10,
                    FirstName = "მამუკა",
                    LastName = "სამხარაძე",
                    PersonalId = "38001011164",
                    PhoneNumber = "597726630",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 11,
                    FirstName = "ნუგზარი",
                    LastName = "სამხარაძე",
                    PersonalId = "38001008077",
                    PhoneNumber = "599868887",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 12,
                    FirstName = "ჯამბული",
                    LastName = "ფავლენიშვილი",
                    PersonalId = "18001063162",
                    PhoneNumber = "555727171",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 13,
                    FirstName = "გიორგი",
                    LastName = "ფხაკაძე",
                    PersonalId = "62001036043",
                    PhoneNumber = "595417709",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 14,
                    FirstName = "ედიშერ",
                    LastName = "ცხვიტაშვილი",
                    PersonalId = "38001035765",
                    PhoneNumber = "571410088",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 15,
                    FirstName = "ლევანი",
                    LastName = "ცხვიტაშვილი",
                    PersonalId = "38001013339",
                    PhoneNumber = "571511331",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 16,
                    FirstName = "დავითი",
                    LastName = "ხუხია",
                    PersonalId = "39001008605",
                    PhoneNumber = "591504051",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 17,
                    FirstName = "ნოდარ",
                    LastName = "ჯიქური",
                    PersonalId = "24001046661",
                    PhoneNumber = "551157052",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 18,
                    FirstName = "ასიფ",
                    LastName = "ვალიევი",
                    PersonalId = "12001008021",
                    PhoneNumber = "579904646",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                }
            );
            modelBuilder.Entity<Company>().HasData(
    new Company { Id = 1, Name = "შპს ირ.აბაშიძე 555 (402204299)" },
    new Company { Id = 2, Name = "შპს ისთ საიდ დეველოფმენტი 405758997" },
    new Company { Id = 3, Name = "შპს ისთ საიდ ლოჯისტიკს (404724937)" },
    new Company { Id = 4, Name = "შპს ისთვისის მენეჯმენტ კომპანი 406374490" },
    new Company { Id = 5, Name = "შპს კ სამი 405382955" },
    new Company { Id = 6, Name = "შპს კ სამი თაუერ 405580198" },
    new Company { Id = 7, Name = "შპს კ სამი ინვესტმენტი (405745234)" },
    new Company { Id = 8, Name = "შპს კავკასუს როუდ პროჯექტ (202258243)" },
    new Company { Id = 9, Name = "შპს კაზა კალდა დეველოფმენტი 404675026" },
    new Company { Id = 10, Name = "შპს კაპიტალ ინვესტი (405307877)" },
    new Company { Id = 11, Name = "შპს კაპიტალ ქონსთრაქშენ საქართველო (406514829)" },
    new Company { Id = 12, Name = "შპს კერა + (205219351)" },
    new Company { Id = 13, Name = "შპს კერა 76 448426454" },
    new Company { Id = 14, Name = "შპს კერა ქონსთრაქშენი 400298316" },
    new Company { Id = 15, Name = "შპს კომპანია ბლექ სი გრუპი (204477734)" },
    new Company { Id = 16, Name = "შპს კონსოლი 405059378" },
    new Company { Id = 17, Name = "შპს კონსთრაქშენ გრუპი 445550216" },
    new Company { Id = 18, Name = "შპს კონცეპტ ჰაუსი(405637244)" },
    new Company { Id = 19, Name = "შპს კორძან ელექტრონიქს 406221921" },
    new Company { Id = 20, Name = "შპს ლაზერი+(402186228)" },
    new Company { Id = 21, Name = "შპს ლაზიკა 416332221" },
    new Company { Id = 22, Name = "შპს ლაიფ ჰაუსი (445478563)" },
    new Company { Id = 23, Name = "შპს ლეგაცი გრუპ (406325285)" },
    new Company { Id = 24, Name = "შპს ლევილი პირველი 445595598" },
    new Company { Id = 25, Name = "შპს ლიდერ ბეტონი (248384449)" },
    new Company { Id = 26, Name = "შპს ლისი არეა (405525668)" },
    new Company { Id = 27, Name = "შპს ლისი თაუერსი 405535915" },
    new Company { Id = 28, Name = "შპს ლიტ ჯეო ინვესტ 243859749" },
    new Company { Id = 29, Name = "შპს ლივიორი (401968082)" },
    new Company { Id = 30, Name = "შპს მ.გ.ფ.გ. (406099270)" },
    new Company { Id = 31, Name = "შპს მალუ კვადრატი 417899180" },
    new Company { Id = 32, Name = "შპს მარნეული რუსთავი 405391428" },
    new Company { Id = 33, Name = "შპს მაურო დეველოფმენტი 405682603" },
    new Company { Id = 34, Name = "შპს მაურო ქონსთრაქშენ (445433120)" },
    new Company { Id = 35, Name = "შპს მბ აგრო 436035559" },
    new Company { Id = 36, Name = "შპს მგს-ს საქართველოს ფილიალი (205168539)" },
    new Company { Id = 37, Name = "შპს მეგა ცენტრი (402192943)" },
    new Company { Id = 38, Name = "შპს მეგა ჰოლდინგი (ყოფილი თბილცემენტ გრუპი)" },
    new Company { Id = 39, Name = "შპს მეტონტაჟი 2014 (415091457)" },
    new Company { Id = 40, Name = "შპს მეონ ინტერნეიშენალ 208213084" },
    new Company { Id = 41, Name = "შპს მურაჟი მოუნთი (405625015)" }
);

            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, Name = "თბილისი, ბაგები წყნეთის გზატკეცილი 40" },
                new Address { Id = 2, Name = "თბილისი, გაზაფხულის ქუჩა 79" },
                new Address { Id = 3, Name = "თბილისი, ევგენია მაისურაძის ქ 7" },
                new Address { Id = 4, Name = "რუსთავი, XVI მკრ." },
                new Address { Id = 5, Name = "თბილისი,ვაშლიჯვარი. მანდორ პეტეფის 54" },
                new Address { Id = 6, Name = "სოფელი დიღომი, ფატმანის 7" },
                new Address { Id = 7, Name = "თბილისი, გურამიშვილის 64" },
                new Address { Id = 8, Name = "თბილისი, ციხისძირის 69 (ნავთლუღი)" },
                new Address { Id = 9, Name = "თბილისი, დიღომი, როსტევანის 1 ჩიხი N3" },
                new Address { Id = 10, Name = "თბილისი, სოფელი დიღომი, ფატმანის 5" },
                new Address { Id = 11, Name = "მცხეთა, ნატახტარი" },
                new Address { Id = 12, Name = "ბათუმი, გრიგოლ ელიავას 41 ა" },
                new Address { Id = 13, Name = "თბილისი,ვახტანგ ჩიქოვანის N2" },
                new Address { Id = 14, Name = "თბილისი,პოლიციის #7" },
                new Address { Id = 15, Name = "თბილისი,ზურაბ ანჯაფარიძის 34" },
                new Address { Id = 16, Name = "თბილისი, ზ.ფალიაშვილის 100" },
                new Address { Id = 17, Name = "ახმეტის რაიონი,სოფ. ალავერდი" },
                new Address { Id = 18, Name = "ბათუმი, აეროპორტის გზატკეცილი 243" },
                new Address { Id = 19, Name = "თბილისი, ცოტნე დადიანის 263" },
                new Address { Id = 20, Name = "გურჯაანი, სოფ არაშენდა" },
                new Address { Id = 21, Name = "თბილისი, ცაცხვების 13;15;17" },
                new Address { Id = 22, Name = "თბილისი, ცაცხვების ქუჩა 15-17; მე-9 შესახვევი" },
                new Address { Id = 23, Name = "ბათუმი, ტბელ აბუსერიძის 23" },
                new Address { Id = 24, Name = "ბათუმი, ტბელ აბუსერიძის 19" },
                new Address { Id = 25, Name = "რუსთავი, თბილისის ქუჩის 30-ის მიმდ." },
                new Address { Id = 26, Name = "ბათუმი, ხახულის 63" },
                new Address { Id = 27, Name = "თბილისი, დიღომის მასივი, III კვარტალი" },
                new Address { Id = 28, Name = "რუსთავი, მოდებაძის ქუჩის მიმდებარედ" },
                new Address { Id = 29, Name = "ბათუმი, ფრიდონ ხალვაშის 5 შესახვევის 13" },
                new Address { Id = 30, Name = "ხელვაჩაური, აკაკი შანიძის 13" },
                new Address { Id = 31, Name = "ხელვაჩაური, ფრ. ხალვაშის 13" },
                new Address { Id = 32, Name = "თბილისი, ენუქიძის 7" },
                new Address { Id = 33, Name = "თბილისი, დ.ბაქრაძის ქ.20ბ" },
                new Address { Id = 34, Name = "ბათუმი, ავ. შანიძის 7" },
                new Address { Id = 35, Name = "თბილისი, ლისი, ინტენსიფიკაციის ქუჩის მიმდ. (ნაკ N11/11)" },
                new Address { Id = 36, Name = "თბილისი, ელგუჯა ამაშუკელის 5" },
                new Address { Id = 37, Name = "თბილისი, ვარკეთილის მე-3 მასივი, მე8 კვ, კორპ17-ის მიმ" },
                new Address { Id = 38, Name = "თბილისი, ხომერულის 27" },
                new Address { Id = 39, Name = "თბილისი, ვაჟა-ფშაველას II კვარტალი N9" },
                new Address { Id = 40, Name = "გურჯაანი, ნიკო ლომოურის ქ.9გ" }, // Fixed overlapping Id indexes
                new Address { Id = 41, Name = "თბილისი, ნოე რამიშვილის ქ 30" },
                new Address { Id = 42, Name = "თბილისი, ნოე რამიშვილის ქ" },
                new Address { Id = 43, Name = "თბილისი, თვალჭრელიძის 35ა" },
                new Address { Id = 44, Name = "გარდაბანი, ჯანდარა, CSG 1 BP პროექტი" },
                new Address { Id = 45, Name = "გარდაბანი, დავით აღმაშენებლის ქ. 34" },
                new Address { Id = 46, Name = "თბილისი, ქიზიყის 28" },
                new Address { Id = 47, Name = "თბილისი, ნიზამიშვილის 262ბ" },
                new Address { Id = 48, Name = "რუსთავი ,ახმეტელის ქ#8 (25-ე საბავშვო ბაღთან)" },
                new Address { Id = 49, Name = "საგარეჯო, სოფელი ლამბალო" },
                new Address { Id = 50, Name = "ფოთი, დანელიას ჟ#84" },
                new Address { Id = 51, Name = "ფოთი, წმინდა გიორგის ქ." },
                new Address { Id = 52, Name = "თბილისი, ქავთარაძის 20" },
                new Address { Id = 53, Name = "გურჯაანი, სოფ.კაჭრეთი" },
                new Address { Id = 54, Name = "გურჯაანი, სოფელი კაჭრეთის მიმდებარედ" }
            );

            modelBuilder.Entity<CompanyAddress>().HasData(
                new CompanyAddress { Id = 1, CompanyId = 1, AddressId = 1 },
                new CompanyAddress { Id = 2, CompanyId = 2, AddressId = 2 },
                new CompanyAddress { Id = 3, CompanyId = 3, AddressId = 3 },
                new CompanyAddress { Id = 4, CompanyId = 4, AddressId = 4 },
                new CompanyAddress { Id = 5, CompanyId = 5, AddressId = 5 },
                new CompanyAddress { Id = 6, CompanyId = 5, AddressId = 6 },
                new CompanyAddress { Id = 7, CompanyId = 6, AddressId = 7 },
                new CompanyAddress { Id = 8, CompanyId = 6, AddressId = 8 },
                new CompanyAddress { Id = 9, CompanyId = 7, AddressId = 9 },
                new CompanyAddress { Id = 10, CompanyId = 7, AddressId = 10 },
                new CompanyAddress { Id = 11, CompanyId = 8, AddressId = 11 },
                new CompanyAddress { Id = 12, CompanyId = 9, AddressId = 12 },
                new CompanyAddress { Id = 13, CompanyId = 9, AddressId = 13 },
                new CompanyAddress { Id = 14, CompanyId = 9, AddressId = 14 },
                new CompanyAddress { Id = 15, CompanyId = 10, AddressId = 15 },
                new CompanyAddress { Id = 16, CompanyId = 11, AddressId = 16 },
                new CompanyAddress { Id = 17, CompanyId = 12, AddressId = 17 },
                new CompanyAddress { Id = 18, CompanyId = 13, AddressId = 18 },
                new CompanyAddress { Id = 19, CompanyId = 14, AddressId = 19 },
                new CompanyAddress { Id = 20, CompanyId = 15, AddressId = 20 },
                new CompanyAddress { Id = 21, CompanyId = 16, AddressId = 21 },
                new CompanyAddress { Id = 22, CompanyId = 16, AddressId = 22 },
                new CompanyAddress { Id = 23, CompanyId = 17, AddressId = 23 },
                new CompanyAddress { Id = 24, CompanyId = 17, AddressId = 24 },
                new CompanyAddress { Id = 25, CompanyId = 18, AddressId = 25 },
                new CompanyAddress { Id = 26, CompanyId = 19, AddressId = 26 },
                new CompanyAddress { Id = 27, CompanyId = 20, AddressId = 27 },
                new CompanyAddress { Id = 28, CompanyId = 21, AddressId = 28 },
                new CompanyAddress { Id = 29, CompanyId = 22, AddressId = 29 },
                new CompanyAddress { Id = 30, CompanyId = 22, AddressId = 30 },
                new CompanyAddress { Id = 31, CompanyId = 22, AddressId = 31 },
                new CompanyAddress { Id = 32, CompanyId = 23, AddressId = 32 },
                new CompanyAddress { Id = 33, CompanyId = 24, AddressId = 33 },
                new CompanyAddress { Id = 34, CompanyId = 25, AddressId = 34 },
                new CompanyAddress { Id = 35, CompanyId = 26, AddressId = 35 },
                new CompanyAddress { Id = 36, CompanyId = 27, AddressId = 36 },
                new CompanyAddress { Id = 37, CompanyId = 28, AddressId = 37 },
                new CompanyAddress { Id = 38, CompanyId = 29, AddressId = 38 },
                new CompanyAddress { Id = 39, CompanyId = 30, AddressId = 39 },
                new CompanyAddress { Id = 40, CompanyId = 31, AddressId = 20 }, // Maps to unique "სოფ არაშენდა"
                new CompanyAddress { Id = 41, CompanyId = 32, AddressId = 40 },
                new CompanyAddress { Id = 42, CompanyId = 33, AddressId = 41 },
                new CompanyAddress { Id = 43, CompanyId = 34, AddressId = 42 },
                new CompanyAddress { Id = 44, CompanyId = 34, AddressId = 41 },
                new CompanyAddress { Id = 45, CompanyId = 35, AddressId = 43 },
                new CompanyAddress { Id = 46, CompanyId = 36, AddressId = 44 },
                new CompanyAddress { Id = 47, CompanyId = 37, AddressId = 45 },
                new CompanyAddress { Id = 48, CompanyId = 38, AddressId = 46 },
                new CompanyAddress { Id = 49, CompanyId = 38, AddressId = 47 },
                new CompanyAddress { Id = 50, CompanyId = 38, AddressId = 48 },
                new CompanyAddress { Id = 51, CompanyId = 38, AddressId = 49 },
                new CompanyAddress { Id = 52, CompanyId = 39, AddressId = 50 },
                new CompanyAddress { Id = 53, CompanyId = 39, AddressId = 51 },
                new CompanyAddress { Id = 54, CompanyId = 40, AddressId = 52 },
                new CompanyAddress { Id = 55, CompanyId = 41, AddressId = 53 },
                new CompanyAddress { Id = 56, CompanyId = 41, AddressId = 54 }
            );
        }
    }
}
