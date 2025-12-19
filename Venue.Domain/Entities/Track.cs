namespace Venue.Domain.Entities;

public class Track
{
    public Guid Id { get; private set; }
    public Guid CallForPapersId { get; private set; } 

    public string Name { get; private set; }        
    public string? Description { get; private set; }

    public Guid? TrackChairUserId { get; private set; }

    public Track(string name, string? description, Guid? trackChairUserId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        TrackChairUserId = trackChairUserId;
    }
}