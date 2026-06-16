using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users")
                .HasKey(u => u.Id);
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(25)
                .IsUnicode(true);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(true);
            builder.Property(u => u.PersonalId)
                .IsRequired()
                .HasMaxLength(11);
            builder.HasIndex(u => u.PersonalId)
                .IsUnique();
            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            builder.HasIndex(u => u.PhoneNumber)
                .IsUnique();
            builder.Property(u => u.CreatedAt)
                .IsRequired();
            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");

            builder.HasMany(u => u.SmsCodes)
                .WithOne(sc => sc.User)
                .HasForeignKey(sc => sc.UserId);
            builder.HasMany(u => u.PumpOperations)
                .WithOne(p => p.Operator)
                .HasForeignKey(p => p.OperatorId);
        }
    }
}
