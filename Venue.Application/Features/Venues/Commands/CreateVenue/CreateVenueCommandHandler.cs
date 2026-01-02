using MediatR;
using Venue.Application.Common.Interfaces;

namespace Venue.Application.Features.Venues.Commands.CreateVenue;

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Guid>
{
    private readonly IVenueDbContext _context;

    public CreateVenueCommandHandler(IVenueDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = new Domain.Entities.Venue(
            request.Name,
            request.Acronym,
            request.Type,
            request.Description
        );

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}