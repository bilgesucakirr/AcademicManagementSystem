using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.RecordDecision;

public class RecordDecisionCommandHandler : IRequestHandler<RecordDecisionCommand, Unit>
{
    private readonly ISubmissionDbContext _context;

    public RecordDecisionCommandHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RecordDecisionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (submission == null)
            throw new KeyNotFoundException("Submission not found.");

        submission.Status = request.Decision;

        var auditEntry = new AuditEvent
        {
            ActorId = "SystemEditor",
            Action = "DecisionRecorded",
            EntityType = "Submission",
            EntityId = submission.Id,
            Metadata = $"Decision: {request.Decision}"
        };

        await _context.AuditEvents.AddAsync(auditEntry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}