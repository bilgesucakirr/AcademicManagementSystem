using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Venue.Domain.Entities;
using Venue.Application.Common.Interfaces;

namespace Venue.Infrastructure.Persistence;

public class VenueDbContext : DbContext, IVenueDbContext
{
    public VenueDbContext(DbContextOptions<VenueDbContext> options) : base(options)
    {
    }

    // Tablolarımız
    public DbSet<Domain.Entities.Venue> Venues { get; set; }
    public DbSet<VenueEdition> VenueEditions { get; set; }
    public DbSet<CallForPapers> CallForPapers { get; set; }
    public DbSet<Track> Tracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Yukarıda yazdığımız "Configurations" klasöründeki tüm ayarları otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}