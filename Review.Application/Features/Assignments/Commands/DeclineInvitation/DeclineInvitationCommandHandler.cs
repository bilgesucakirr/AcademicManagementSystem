using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;

namespace Review.Application.Features.Assignments.Commands.DeclineInvitation;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeclineInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(x => x.Id == request.AssignmentId, cancellationToken);

        if (assignment == null)
            throw new KeyNotFoundException($"Assignment {request.AssignmentId} not found.");

        // Domain metodunu çağırıyoruz (Durumu 'Declined' yapar)
        assignment.DeclineInvitation(request.Reason);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}