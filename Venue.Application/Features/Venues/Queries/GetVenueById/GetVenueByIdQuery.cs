using MediatR;
using Microsoft.EntityFrameworkCore;
using Venue.Application.Common.Interfaces;

namespace Venue.Application.Features.Venues.Queries.GetVenueById;

public record GetVenueByIdQuery(Guid Id) : IRequest<VenueDetailDto>;

public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDetailDto>
{
    private readonly IVenueDbContext _context;

    public GetVenueByIdQueryHandler(IVenueDbContext context)
    {
        _context = context;
    }

    public async Task<VenueDetailDto> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
    {
        var venue = await _context.Venues
            .Include(v => v.Editions)
                .ThenInclude(e => e.CallForPapers)
                    .ThenInclude(c => c.Tracks)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (venue == null) return null!;

        var activeEdition = venue.Editions.FirstOrDefault(e => e.IsActive);
        var activeCfp = activeEdition?.CallForPapers.FirstOrDefault();

        return new VenueDetailDto
        {
            Id = venue.Id,
            Name = venue.Name,
            ActiveEditionId = activeEdition?.Id ?? Guid.Empty,
            ActiveCfpId = activeCfp?.Id ?? Guid.Empty,
            Tracks = activeCfp?.Tracks.Select(t => new TrackDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList() ?? new()
        };
    }
}