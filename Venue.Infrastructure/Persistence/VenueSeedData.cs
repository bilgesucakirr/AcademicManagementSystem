using Venue.Domain.Entities;
using Venue.Domain.Enums;

namespace Venue.Infrastructure.Persistence;

public static class VenueSeedData
{
    public static async Task SeedAsync(VenueDbContext context)
    {
        if (context.Venues.Any()) return;

        var conference = new Domain.Entities.Venue(
            "International Software Architecture Conference",
            "ISAC",
            VenueType.Conference,
            "Leading global conference on software architecture.",
            "Software,Architecture,Cloud",
            null,
            "admin@jarvis.com"
        );

        var confEdition = new VenueEdition(conference.Id, "2025 Edition", DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        var confCfp = new CallForPapers(confEdition.Id, "Main Track", "Submissions open.", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), BlindMode.DoubleBlind);

        confCfp.Open();
        confCfp.AddTrack("General Track", "General submissions.", null);
        confEdition.CallForPapers.Add(confCfp);
        conference.Editions.Add(confEdition);

        await context.Venues.AddAsync(conference);
        await context.SaveChangesAsync();
    }
}