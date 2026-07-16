using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class CompanyAddressConfiguration : IEntityTypeConfiguration<CompanyAddress>
    {
        public void Configure(EntityTypeBuilder<CompanyAddress> builder)
        {
            builder.ToTable("CompanyAddresses")
                .HasKey(e => e.Id);
            builder.Property(e => e.CompanyId)
                .IsRequired();
            builder.Property(e => e.AddressId)
                .IsRequired();
            builder.HasIndex(e => new { e.CompanyId, e.AddressId })
                .IsUnique();

            builder.HasOne(ca => ca.Company)
                .WithMany(c => c.CompanyAddresses)
                .HasForeignKey(ca => ca.CompanyId);
            builder.HasOne(ca => ca.Address)
                .WithMany(a => a.CompanyAddresses)
                .HasForeignKey(ca => ca.AddressId);
        }
    }
}
