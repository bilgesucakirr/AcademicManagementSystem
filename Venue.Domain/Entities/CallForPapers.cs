using Venue.Domain.Enums;

namespace Venue.Domain.Entities;

public class CallForPapers
{
    public Guid Id { get; private set; }
    public Guid VenueEditionId { get; private set; } // Hangi yıla/baskıya ait?

    public string Title { get; private set; }       // Örn: "Main Track Submission"
    public string Description { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime Deadline { get; private set; }

    public BlindMode BlindMode { get; private set; } // Kör hakemlik kuralı
    public CfpStatus Status { get; private set; }

    // Navigation: Bu çağrının konu başlıkları
    public ICollection<Track> Tracks { get; private set; } = new List<Track>();

    public CallForPapers(Guid venueEditionId, string title, DateTime start, DateTime deadline, BlindMode blindMode)
    {
        Id = Guid.NewGuid();
        VenueEditionId = venueEditionId;
        Title = title;
        StartDate = start;
        Deadline = deadline;
        BlindMode = blindMode;
        Status = CfpStatus.Draft; // Varsayılan taslak olarak başlar
    }

    // İş Kuralları (Business Logic)
    public void Open() => Status = CfpStatus.Open;
    public void Close() => Status = CfpStatus.Closed;

    public void AddTrack(string name, string? desc, Guid? chairId)
    {
        Tracks.Add(new Track(name, desc, chairId));
    }
}