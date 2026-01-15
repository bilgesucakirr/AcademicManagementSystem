using MediatR;

namespace Venue.Application.Features.Venues.Commands.CreateVenue;

public record CreateVenueCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Type { get; set; } = "Conference";
    public string AimAndScope { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string? OrganizerEmail { get; set; } // Bu alanın burada olduğundan emin olun
    public string? ReviewFormUrl { get; set; }
    public List<string> Tracks { get; set; } = new();
}