using Venue.Domain.Enums;

namespace Venue.Domain.Entities;

public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }     // Örn: "International Software Conf."
    public string? Acronym { get; private set; } // Örn: "ISC"
    public VenueType Type { get; private set; }
    public string Description { get; private set; }

    // Geçmiş ve gelecek tüm baskılar
    public ICollection<VenueEdition> Editions { get; private set; } = new List<VenueEdition>();

    public Venue(string name, string? acronym, VenueType type, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Acronym = acronym;
        Type = type;
        Description = description;
    }
}