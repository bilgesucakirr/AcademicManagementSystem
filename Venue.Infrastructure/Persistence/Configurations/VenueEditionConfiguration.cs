using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venue.Domain.Entities;

namespace Venue.Infrastructure.Persistence.Configurations;

public class VenueEditionConfiguration : IEntityTypeConfiguration<VenueEdition>
{
    public void Configure(EntityTypeBuilder<VenueEdition> builder)
    {
        builder.ToTable("VenueEditions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        // Edition silinirse altındaki Çağrılar (CFP) da silinsin
        builder.HasMany(e => e.CallForPapers)
            .WithOne()
            .HasForeignKey(c => c.VenueEditionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}