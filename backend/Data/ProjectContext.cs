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

            // seeding
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "ნიკოლოზ",
                    LastName = "ლორთქიფანიძე",
                    PersonalId = "12345678901",
                    PhoneNumber = "575780323",
                    CreatedAt = DateTime.Parse("2026-05-28T12:00:00Z")
                }    
            );
        }
    }
}
