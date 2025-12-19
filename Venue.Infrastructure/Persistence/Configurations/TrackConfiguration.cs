using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Venue.Domain.Entities;

namespace Venue.Infrastructure.Persistence.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Tracks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);

        // TrackChairUserId opsiyonel olabilir, veritabanında indexleyelim ki araması hızlı olsun
        builder.HasIndex(t => t.TrackChairUserId);
    }
}