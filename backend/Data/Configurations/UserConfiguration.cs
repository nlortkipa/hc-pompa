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
            builder.Property(u => u.Number)
                .IsRequired()
                .HasMaxLength(15);
            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.HasMany(u => u.SmsCodes)
                .WithOne(sc => sc.User)
                .HasForeignKey(sc => sc.UserId);
        }
    }
}
