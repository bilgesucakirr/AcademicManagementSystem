namespace Venue.Application.Features.Venues.Queries.GetVenueById;

public class VenueDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? OrganizerEmail { get; set; }

    public string? ReviewFormUrl { get; set; }
    public Guid ActiveEditionId { get; set; }
    public Guid ActiveCfpId { get; set; }
    public List<TrackDto> Tracks { get; set; } = new();
}

public class TrackDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}