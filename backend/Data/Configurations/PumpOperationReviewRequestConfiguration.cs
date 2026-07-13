using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class PumpOperationReviewRequestConfiguration : IEntityTypeConfiguration<PumpOperationReviewRequest>
    {
        public void Configure(EntityTypeBuilder<PumpOperationReviewRequest> builder)
        {
            builder.ToTable("PumpOperationReviewRequests")
                .HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.CheckerGmail)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasIndex(r => r.Token)
                .IsUnique();

            builder.HasOne(r => r.PumpOperation)
                .WithMany(p => p.ReviewRequests)
                .HasForeignKey(r => r.PumpOperationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
