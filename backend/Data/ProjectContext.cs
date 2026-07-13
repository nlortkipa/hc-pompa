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
        public DbSet<Plate> Plates { get; set; }
        public DbSet<PumpOperationReviewRequest> PumpOperationReviewRequests { get; set; }

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
            modelBuilder.ApplyConfiguration(new PlateConfiguration());
            modelBuilder.ApplyConfiguration(new PumpOperationReviewRequestConfiguration());

            // seeding
            modelBuilder.Entity<Plate>().HasData(
                new Plate { Id = 1, Number = "II210NI", CheckerGmail = "" },
                new Plate { Id = 2, Number = "PB107PB", CheckerGmail = "" },
                new Plate { Id = 3, Number = "BB685FF", CheckerGmail = "" },
                new Plate { Id = 4, Number = "VQ632QV", CheckerGmail = "" },
                new Plate { Id = 5, Number = "CC178LC", CheckerGmail = "" },
                new Plate { Id = 6, Number = "WW832GW", CheckerGmail = "" },
                new Plate { Id = 7, Number = "OV247VO", CheckerGmail = "" }
            );

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
                    PersonalId = "01001055244",
                    PhoneNumber = "551710931",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 3,
                    FirstName = "გიორგი",
                    LastName = "გუჯეჯიანი",
                    PersonalId = "01005027479",
                    PhoneNumber = "598606823",
                    CreatedAt = DateTime.Parse("2026-06-16T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 4,
                    FirstName = "დათო",
                    LastName = "ელგენდარაშვილი",
                    PersonalId = "01008045338",
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
            modelBuilder.Entity<Company>().HasData(CompanyAddressSeedData.Companies);
            modelBuilder.Entity<Address>().HasData(CompanyAddressSeedData.Addresses);
            modelBuilder.Entity<CompanyAddress>().HasData(CompanyAddressSeedData.CompanyAddresses);
        }
    }
}
