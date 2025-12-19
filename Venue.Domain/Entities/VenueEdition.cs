using Venue.Domain.Entities;

namespace Venue.Domain.Entities;

public class VenueEdition
{
    public Guid Id { get; private set; }
    public Guid VenueId { get; private set; }

    public string Name { get; private set; } // "2025" veya "Vol 3 Issue 2"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<CallForPapers> CallForPapers { get; private set; } = new List<CallForPapers>();

   
    private VenueEdition() { }

    public VenueEdition(Guid venueId, string name, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        VenueId = venueId;
        Name = name;
        StartDate = startDate; // Artık property ismiyle eşleşiyor
        EndDate = endDate;     // Artık property ismiyle eşleşiyor
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
}