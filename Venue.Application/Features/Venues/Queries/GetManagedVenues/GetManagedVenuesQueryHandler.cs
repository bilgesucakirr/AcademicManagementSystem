using MediatR;
using Microsoft.EntityFrameworkCore;
using Venue.Application.Common.Interfaces;
using Venue.Application.Features.Venues.Queries.GetAllVenues;

namespace Venue.Application.Features.Venues.Queries.GetManagedVenues;

public class GetManagedVenuesQueryHandler : IRequestHandler<GetManagedVenuesQuery, List<VenueDto>>
{
    private readonly IVenueDbContext _context;

    public GetManagedVenuesQueryHandler(IVenueDbContext context)
    {
        _context = context;
    }

    public async Task<List<VenueDto>> Handle(GetManagedVenuesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Venues
            .Where(v => v.OrganizerEmail == request.Email)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Acronym = v.Acronym ?? string.Empty,
                Type = v.Type.ToString(),
                Description = v.Description
            })
            .ToListAsync(cancellationToken);
    }
}