using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review.Domain.Entities;

namespace Review.Infrastructure.Persistence.Configurations;

public class ReviewAssignmentConfiguration : IEntityTypeConfiguration<ReviewAssignment>
{
    public void Configure(EntityTypeBuilder<ReviewAssignment> builder)
    {
        builder.ToTable("ReviewAssignments"); 
        builder.HasKey(ra => ra.Id);  
        builder.Property(ra => ra.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(ra => ra.Review)
               .WithOne(r => r.Assignment)
               .HasForeignKey<Domain.Entities.Review>(r => r.AssignmentId);

        builder.HasIndex(ra => new { ra.SubmissionId, ra.ReviewerUserId }).IsUnique();
    }
}