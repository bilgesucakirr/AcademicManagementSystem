using Venue.Domain.Entities;
using Venue.Domain.Enums;

namespace Venue.Infrastructure.Persistence;

public static class VenueSeedData
{
    public static async Task SeedAsync(VenueDbContext context)
    {
        
        if (context.Venues.Any())
        {
            return;
        }

        var conference = new Domain.Entities.Venue(
            "International Software Architecture Conference",
            "ISAC",
            VenueType.Conference,
            "The leading global conference on software architecture and design patterns."
        );

        var confEdition = new VenueEdition(
            conference.Id,
            "2025 - Istanbul",
            new DateTime(2025, 10, 15),
            new DateTime(2025, 10, 18)
        );

        var confCfp = new CallForPapers(
            confEdition.Id,
            "Main Track Submissions",
            "We invite original research papers on all aspects of software architecture.", 
            DateTime.UtcNow,                
            DateTime.UtcNow.AddMonths(2),   
            BlindMode.DoubleBlind           
        );

        confCfp.Open(); 

        confCfp.AddTrack("Microservices & Cloud", "Architectural patterns for distributed systems.", null);
        confCfp.AddTrack("AI Engineering", "Software engineering for AI-based systems.", null);
        confCfp.AddTrack("Legacy Modernization", "Strategies for migrating legacy systems.", null);

       
        confEdition.CallForPapers.Add(confCfp);
        conference.Editions.Add(confEdition);

        var journal = new Domain.Entities.Venue(
            "Journal of Modern Computing",
            "JMC",
            VenueType.Journal,
            "A quarterly peer-reviewed journal covering all aspects of computer science."
        );

        var journalIssue = new VenueEdition(
            journal.Id,
            "Vol 12, Issue 3",
            new DateTime(2025, 01, 01),
            new DateTime(2025, 03, 30)
        );

        var journalCfp = new CallForPapers(
            journalIssue.Id,
            "General Submission",
            "Open for submissions on general computer science topics.", 
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1), 
            BlindMode.SingleBlind        
        );

        journalCfp.Open();

        journalIssue.CallForPapers.Add(journalCfp);
        journal.Editions.Add(journalIssue);

        await context.Venues.AddRangeAsync(conference, journal);
        await context.SaveChangesAsync();
    }
}