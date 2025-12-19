using Microsoft.EntityFrameworkCore;
using Venue.Domain.Entities;

namespace Venue.Application.Common.Interfaces;

public interface IVenueDbContext
{
    DbSet<Venue.Domain.Entities.Venue> Venues { get; }
    DbSet<VenueEdition> VenueEditions { get; }
    DbSet<CallForPapers> CallForPapers { get; }
    DbSet<Track> Tracks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}