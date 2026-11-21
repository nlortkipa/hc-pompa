using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class PlateCheckerConfiguration : IEntityTypeConfiguration<PlateChecker>
    {
        public void Configure(EntityTypeBuilder<PlateChecker> builder)
        {
            builder.ToTable("PlateCheckers");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.PlateId, x.UserId }).IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(u => u.CheckedPlates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
