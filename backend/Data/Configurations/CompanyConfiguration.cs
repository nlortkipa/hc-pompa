using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies")
                .HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(true);

            builder.HasMany(c => c.CompanyAddresses)
                .WithOne(ca => ca.Company)
                .HasForeignKey(ca => ca.CompanyId);
            builder.HasMany(c => c.PumpOperations)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId);
        }
    }
}
