using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configurations
{
    public class PlateConfiguration : IEntityTypeConfiguration<Plate>
    {
        public void Configure(EntityTypeBuilder<Plate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Number).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(20);

            builder.HasMany(x => x.Checkers)
                .WithOne(x => x.Plate)
                .HasForeignKey(x => x.PlateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
