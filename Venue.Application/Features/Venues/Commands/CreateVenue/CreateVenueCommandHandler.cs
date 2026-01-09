using MediatR;
using Venue.Application.Common.Interfaces;
using Venue.Domain.Entities;
using Venue.Domain.Enums;

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
        if (!Enum.TryParse<VenueType>(request.Type, true, out var venueType))
        {
            throw new ArgumentException($"Invalid Venue Type: {request.Type}");
        }

        // 1. Venue (Konferans/Dergi) Oluştur
        var venue = new Domain.Entities.Venue(
            request.Name,
            request.Acronym,
            venueType,
            request.Description
        );

        // 2. Varsayılan Bir Edition (Baskı/Dönem) Oluştur (Örn: 2025)
        var editionName = $"{DateTime.UtcNow.Year} Edition";
        var edition = new VenueEdition(
            venue.Id,
            editionName,
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1)
        );

        // 3. Varsayılan Çağrı (Call For Papers) Oluştur
        var cfp = new CallForPapers(
            edition.Id,
            "General Submission",
            "Open for all topics related to the venue scope.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
            BlindMode.SingleBlind
        );
        cfp.Open();

     
        cfp.AddTrack("General Track", "General submissions.", null);

        edition.CallForPapers.Add(cfp);
        venue.Editions.Add(edition);

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}