using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;
using Review.Domain.Enums;

namespace Review.Application.Features.Assignments.Commands.ReActivateAssignments;

public record ReActivateAssignmentsCommand(Guid SubmissionId) : IRequest<Unit>;

public class ReActivateAssignmentsCommandHandler : IRequestHandler<ReActivateAssignmentsCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ReActivateAssignmentsCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ReActivateAssignmentsCommand request, CancellationToken cancellationToken)
    {
        var assignments = await _context.ReviewAssignments
            .Where(a => a.SubmissionId == request.SubmissionId && a.Status == ReviewAssignmentStatus.Submitted)
            .ToListAsync(cancellationToken);

        foreach (var assignment in assignments)
        {
            assignment.ReActivate();

            try
            {
                await _emailService.SendRevisionNotificationAsync(assignment.ReviewerEmail, "New Revision Uploaded");
            }
            catch { }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}