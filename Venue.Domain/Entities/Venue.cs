using Venue.Domain.Enums;

namespace Venue.Domain.Entities;

public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Acronym { get; private set; }
    public VenueType Type { get; private set; }
    public string Description { get; private set; }
    public string Keywords { get; private set; } = string.Empty;
    public string? ReviewFormUrl { get; private set; }
    public string? OrganizerEmail { get; private set; }

    public ICollection<VenueEdition> Editions { get; private set; } = new List<VenueEdition>();

    private Venue() { }

    public Venue(string name, string? acronym, VenueType type, string description, string keywords, string? reviewFormUrl, string? organizerEmail)
    {
        Id = Guid.NewGuid();
        Name = name;
        Acronym = acronym;
        Type = type;
        Description = description;
        Keywords = keywords;
        ReviewFormUrl = reviewFormUrl;
        OrganizerEmail = organizerEmail;
    }
}