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
        public DbSet<LicensePlatePermission> LicensePlatePermissions { get; set; }
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
            modelBuilder.ApplyConfiguration(new LicensePlatePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new PumpOperationConfiguration());

            // seeding
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "ჯემალ",
                    LastName = "გიორგობიანი",
                    PersonalId = "1001055244",
                    PhoneNumber = "551710931",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    FirstName = "გიორგი",
                    LastName = "გუჯეჯიანი",
                    PersonalId = "01005027479",
                    PhoneNumber = "598606823",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 3,
                    FirstName = "დათო",
                    LastName = "ელგენდარაშვილი",
                    PersonalId = "01008045338",
                    PhoneNumber = "01008045338",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 4,
                    FirstName = "ზვიადი",
                    LastName = "ენუქიძე",
                    PersonalId = "38001031038",
                    PhoneNumber = "591207381",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z"),
                    Role = "User"
                },
                new User
                {
                    Id = 5,
                    FirstName = "ნიკოლოზ",
                    LastName = "ლორთქიფანიძე",
                    PersonalId = "11111111111",
                    PhoneNumber = "575780323",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z"),
                    Role = "User"
                }
            );
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Company 1"
                },
                new Company
                {
                    Id = 2,
                    Name = "Company 2"
                },
                new Company
                {
                    Id = 3,
                    Name = "Company 3"
                }
            );
            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    Id = 1,
                    Name = "Address 1"
                },
                new Address
                {
                    Id = 2,
                    Name = "Address 2"
                },
                new Address
                {
                    Id = 3,
                    Name = "Address 3"
                },
                new Address
                {
                    Id = 4,
                    Name = "Address 4"
                },
                new Address
                {
                    Id = 5,
                    Name = "Address 5"
                },
                new Address
                {
                    Id = 6,
                    Name = "Address 6"
                }
            );
            modelBuilder.Entity<CompanyAddress>().HasData(
                new CompanyAddress
                {
                    Id = 1,
                    CompanyId = 1,
                    AddressId = 1
                },
                new CompanyAddress
                {
                    Id = 2,
                    CompanyId = 1,
                    AddressId = 2
                },
                new CompanyAddress
                {
                    Id = 3,
                    CompanyId = 2,
                    AddressId = 3
                },
                new CompanyAddress
                {
                    Id = 4,
                    CompanyId = 2,
                    AddressId = 4
                },
                new CompanyAddress
                {
                    Id = 5,
                    CompanyId = 3,
                    AddressId = 5
                },
                new CompanyAddress
                {
                    Id = 6,
                    CompanyId = 3,
                    AddressId = 6
                }
            );
            modelBuilder.Entity<LicensePlatePermission>().HasData(
                new LicensePlatePermission
                {
                    Id = 1,
                    LicensePlate = "II210NI",
                    Permission = 52
                },
                new LicensePlatePermission
                {
                    Id = 2,
                    LicensePlate = "PB107PB",
                    Permission = 42
                },
                new LicensePlatePermission
                {
                    Id = 3,
                    LicensePlate = "BB685FF",
                    Permission = 42
                },
                new LicensePlatePermission
                {
                    Id = 4,
                    LicensePlate = "VQ632QV",
                    Permission = 42
                },
                new LicensePlatePermission
                {
                    Id = 5,
                    LicensePlate = "CG368CG",
                    Permission = 48
                },
                new LicensePlatePermission
                {
                    Id = 6,
                    LicensePlate = "CC178LC",
                    Permission = 60
                },
                new LicensePlatePermission
                {
                    Id = 7,
                    LicensePlate = "WW832GW",
                    Permission = 60
                },
                new LicensePlatePermission
                {
                    Id = 8,
                    LicensePlate = "OV247VO",
                    Permission = 47
                }
            );
        }
    }
}
