using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class PlateCheckerEmailConfiguration : IEntityTypeConfiguration<PlateCheckerEmail>
    {
        public void Configure(EntityTypeBuilder<PlateCheckerEmail> builder)
        {
            builder.ToTable("PlateCheckerEmails");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(120);
            builder.HasIndex(x => new { x.PlateId, x.Email }).IsUnique();
        }
    }
}
