using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class LicensePlatePermissionConfiguration : IEntityTypeConfiguration<LicensePlatePermission>
    {
        public void Configure(EntityTypeBuilder<LicensePlatePermission> builder)
        {
            builder.ToTable("LicensePlatePermissions")
                .HasKey(l => l.Id);

            builder.Property(l => l.LicensePlate)
                .IsRequired()
                .HasMaxLength(10);
            builder.HasIndex(l => l.LicensePlate)
                .IsUnique();
            builder.Property(l => l.Permission)
                .IsRequired();
        }
    }
}
