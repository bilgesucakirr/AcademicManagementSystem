using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Domain.Entities.Review>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Review> builder)
    {
        builder.ToTable("Reviews"); 
        builder.HasKey(r => r.Id); 

        builder.Property(r => r.OverallScore).HasColumnType("decimal(3, 1)");

        builder.Property(r => r.CommentsToAuthor).IsRequired();
    }
}