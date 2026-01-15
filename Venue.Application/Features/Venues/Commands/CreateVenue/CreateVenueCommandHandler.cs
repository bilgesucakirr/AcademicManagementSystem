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

        // HATA BURADAYDI: Sınıf adını tam yoluyla (Venue.Domain.Entities.Venue) yazıyoruz
        var venue = new Venue.Domain.Entities.Venue(
            request.Name,
            request.Acronym,
            venueType,
            request.AimAndScope,
            request.Keywords,
            request.ReviewFormUrl,
            request.OrganizerEmail
        );

        var edition = new VenueEdition(
            venue.Id,
            $"{DateTime.UtcNow.Year} Edition",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1)
        );

        var cfp = new CallForPapers(
            edition.Id,
            "General Submission",
            "Open for submissions related to the venue scope.",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
            BlindMode.SingleBlind
        );
        cfp.Open();

        if (request.Tracks != null && request.Tracks.Any())
        {
            foreach (var trackName in request.Tracks)
            {
                cfp.AddTrack(trackName, null, null);
            }
        }

        edition.CallForPapers.Add(cfp);

        // Burada da isim çakışması olmaması için koleksiyon üzerinden gidiyoruz
        venue.Editions.Add(edition);

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}