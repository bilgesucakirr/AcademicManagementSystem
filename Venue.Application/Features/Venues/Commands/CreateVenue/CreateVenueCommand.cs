using MediatR;

namespace Venue.Application.Features.Venues.Commands.CreateVenue;

public record CreateVenueCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}