using Venue.Domain.Enums;

namespace Venue.Domain.Entities;

public class CallForPapers
{
    public Guid Id { get; private set; }
    public Guid VenueEditionId { get; private set; }

    public string Title { get; private set; }
    public string Description { get; private set; } // Zorunlu alan

    public DateTime StartDate { get; private set; }
    public DateTime Deadline { get; private set; }

    public BlindMode BlindMode { get; private set; }
    public CfpStatus Status { get; private set; }

    public ICollection<Track> Tracks { get; private set; } = new List<Track>();

    // EF Core için boş constructor
    private CallForPapers() { }

    // GÜNCELLEME: 'description' parametresi eklendi
    public CallForPapers(Guid venueEditionId, string title, string description, DateTime startDate, DateTime deadline, BlindMode blindMode)
    {
        Id = Guid.NewGuid();
        VenueEditionId = venueEditionId;
        Title = title;
        Description = description; // Artık nesne oluşurken atanıyor
        StartDate = startDate;
        Deadline = deadline;
        BlindMode = blindMode;
        Status = CfpStatus.Draft;
    }

    public void Open() => Status = CfpStatus.Open;
    public void Close() => Status = CfpStatus.Closed;

    public void AddTrack(string name, string? desc, Guid? chairId)
    {
        Tracks.Add(new Track(name, desc, chairId));
    }
}