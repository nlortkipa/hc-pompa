using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses")
                .HasKey(a => a.Id);
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);
            builder.HasIndex(a => a.Id)
                .IsUnique();

            builder.HasMany(a => a.CompanyAddresses)
                .WithOne(ca => ca.Address)
                .HasForeignKey(ca => ca.AddressId);
            builder.HasMany(a => a.PumpOperations)
                .WithOne(p => p.Address)
                .HasForeignKey(p => p.AddressId);
        }
    }
}
