using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venue.Domain.Entities;

namespace Venue.Infrastructure.Persistence.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Domain.Entities.Venue>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Venue> builder)
    {
        builder.ToTable("Venues");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.Acronym).HasMaxLength(50);

        // Enum'ı String olarak kaydet (Conference, Journal)
        builder.Property(v => v.Type)
            .HasConversion<string>()
            .IsRequired();

        // Venue silinirse altındaki Edition'lar da silinsin (Cascade)
        builder.HasMany(v => v.Editions)
            .WithOne()
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}