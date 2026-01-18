using MediatR;
using Venue.Application.Features.Venues.Queries.GetAllVenues;

namespace Venue.Application.Features.Venues.Queries.GetManagedVenues;

public record GetManagedVenuesQuery(string Email) : IRequest<List<VenueDto>>;