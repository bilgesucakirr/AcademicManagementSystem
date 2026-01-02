using MediatR;
using Venue.Application.Common.Interfaces;
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
        // String -> Enum dönüşümü ve Validasyon
        if (!Enum.TryParse<VenueType>(request.Type, true, out var venueType))
        {
            // Eğer geçersiz bir tip gelirse (örn: "Seminer") hata fırlat
            throw new ArgumentException($"Invalid Venue Type: {request.Type}. Allowed values: Conference, Journal");
        }

        var venue = new Domain.Entities.Venue(
            request.Name,
            request.Acronym,
            venueType, // Çevrilmiş enum'ı kullanıyoruz
            request.Description
        );

        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}