using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review.Domain.Entities;
using Review.Domain.Enums;

namespace Review.Infrastructure.Persistence.Configurations;

public class ReviewAssignmentConfiguration : IEntityTypeConfiguration<ReviewAssignment>
{
    public void Configure(EntityTypeBuilder<ReviewAssignment> builder)
    {
        builder.ToTable("ReviewAssignments");
        builder.HasKey(ra => ra.Id);

        // Enum'ı String olarak kaydet (Invited, Accepted, vs.)
        builder.Property(ra => ra.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        // Yeni eklediğimiz alanlar
        builder.Property(ra => ra.DeclineReason).HasMaxLength(500);

        // Tarih alanları
        builder.Property(ra => ra.InvitedAt).IsRequired();
        builder.Property(ra => ra.DueAt).IsRequired();

        // İlişki (SubmissionId ve ReviewerUserId için unique index - Aynı kişiye aynı makale iki kere açık atanamasın)
        // Ancak statüsü Declined olanlar bu kuralı bozabilir, o yüzden filtreli index daha iyi olur ama şimdilik basit tutalım.
        builder.HasIndex(ra => new { ra.SubmissionId, ra.ReviewerUserId }).IsUnique(false); // Unique'i kaldırdım çünkü reddedip tekrar atanabilir.

        builder.HasOne(ra => ra.Review)
               .WithOne(r => r.Assignment)
               .HasForeignKey<Review.Domain.Entities.Review>(r => r.AssignmentId);
    }
}