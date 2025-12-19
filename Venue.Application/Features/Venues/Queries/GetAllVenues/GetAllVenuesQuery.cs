using MediatR;
using Microsoft.EntityFrameworkCore;
using Venue.Application.Common.Interfaces;

namespace Venue.Application.Features.Venues.Queries.GetAllVenues;

public record GetAllVenuesQuery : IRequest<List<VenueDto>>;

public class GetAllVenuesQueryHandler : IRequestHandler<GetAllVenuesQuery, List<VenueDto>>
{
    private readonly IVenueDbContext _context;

    public GetAllVenuesQueryHandler(IVenueDbContext context)
    {
        _context = context;
    }

    public async Task<List<VenueDto>> Handle(GetAllVenuesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Venues
            .AsNoTracking()
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Acronym = v.Acronym ?? "",
                Type = v.Type.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}