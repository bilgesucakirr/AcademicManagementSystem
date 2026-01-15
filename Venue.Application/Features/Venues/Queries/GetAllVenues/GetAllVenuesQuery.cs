using MediatR;
using Microsoft.EntityFrameworkCore;
using Venue.Application.Common.Interfaces;
using Venue.Domain.Entities;
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
        var venues = await _context.Venues
            .Include(v => v.Editions)
                .ThenInclude(e => e.CallForPapers)
                    .ThenInclude(c => c.Tracks)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return venues.Select(v => new VenueDto
        {
            Id = v.Id,
            Name = v.Name,
            Acronym = v.Acronym ?? "",
            Type = v.Type.ToString(),
            Description = v.Description,
            Keywords = v.Keywords,
            Tracks = v.Editions.OrderByDescending(e => e.StartDate)
                      .FirstOrDefault()?.CallForPapers.FirstOrDefault()?.Tracks.Select(t => t.Name).ToList() ?? new List<string>()
        }).ToList();
    }
}