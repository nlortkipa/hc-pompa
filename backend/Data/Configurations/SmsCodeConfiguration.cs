using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class SmsCodeConfiguration : IEntityTypeConfiguration<SmsCode>
    {
        public void Configure(EntityTypeBuilder<SmsCode> builder)
        {
            builder.ToTable("SmsCodes")
                .HasKey(sc => sc.Id);
            builder.Property(sc => sc.UserId)
                .IsRequired();
            builder.Property(sc => sc.Code)
                .IsRequired()
                .HasMaxLength(4);
            builder.Property(sc => sc.ExpiresAt)
                .IsRequired();

            builder.HasOne(sc => sc.User)
                .WithMany(u => u.SmsCodes)
                .HasForeignKey(sc => sc.UserId);
        }
    }
}
