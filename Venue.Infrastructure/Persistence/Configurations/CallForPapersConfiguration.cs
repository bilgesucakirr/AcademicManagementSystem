using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venue.Domain.Entities;

namespace Venue.Infrastructure.Persistence.Configurations;

public class CallForPapersConfiguration : IEntityTypeConfiguration<CallForPapers>
{
    public void Configure(EntityTypeBuilder<CallForPapers> builder)
    {
        builder.ToTable("CallForPapers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);

        // Enum -> String dönüşümleri
        builder.Property(c => c.BlindMode).HasConversion<string>().IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().IsRequired();

        // CFP silinirse Track'ler de silinsin
        builder.HasMany(c => c.Tracks)
            .WithOne()
            .HasForeignKey(t => t.CallForPapersId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}