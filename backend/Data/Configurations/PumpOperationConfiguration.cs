using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class PumpOperationConfiguration : IEntityTypeConfiguration<PumpOperation>
    {
        public void Configure(EntityTypeBuilder<PumpOperation> builder)
        {
            builder.ToTable("PumpOperations")
                .HasKey(p => p.Id);
            
            builder.Property(p => p.Date)
                .HasColumnType("date")
                .IsRequired();
            builder.Property(p => p.OperatorId)
                .IsRequired();
            builder.Property(p => p.CompanyId)
                .IsRequired();
            builder.Property(p => p.AddressId)
                .IsRequired();
            builder.Property(p => p.ConcreteVolume)
                .IsRequired();
            builder.Property(p => p.PumpRelocation)
                .IsRequired();
            builder.Property(p => p.StationaryRelocation)
                .IsRequired();
            builder.Property(p => p.HydraulicRelocation)
                .IsRequired();
            builder.Property(p => p.StationaryReposition)
                .IsRequired();
            builder.Property(p => p.IdleTime)
                .IsRequired();
            builder.Property(p => p.DismantleAssemble)
                .IsRequired();
            builder.Property(p => p.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.ReviewStatus)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.ReviewedByCheckerGmail)
                .HasMaxLength(120);

            builder.HasOne(p => p.Operator)
                .WithMany(o => o.PumpOperations)
                .HasForeignKey(p => p.OperatorId);
            builder.HasOne(p => p.Company)
                .WithMany(c => c.PumpOperations)
                .HasForeignKey(p => p.CompanyId);
            builder.HasOne(p => p.Address)
                .WithMany(a => a.PumpOperations)
                .HasForeignKey(p => p.AddressId);
        }
    }
}
