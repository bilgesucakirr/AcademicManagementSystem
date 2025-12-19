using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;
using Review.Domain.Entities;

namespace Review.Application.Features.Assignments.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public AcceptInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(x => x.Id == request.AssignmentId, cancellationToken);

        if (assignment == null)
            throw new KeyNotFoundException($"Assignment {request.AssignmentId} not found.");

        // Burada bir güvenlik kontrolü (Resource-Based Auth) normalde gerekir.
        // "Bu işlemi yapan kullanıcı == assignment.ReviewerUserId mi?"
        // Bunu Controller seviyesinde veya burada User ID'yi inject ederek yapabiliriz.

        // Domain metodunu çağırıyoruz (Durumu 'Accepted' yapar)
        assignment.AcceptInvitation();

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}