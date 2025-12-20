using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.RecordDecision;

public class RecordDecisionCommandHandler : IRequestHandler<RecordDecisionCommand, Unit>
{
    private readonly ISubmissionDbContext _context;
    private readonly IConfiguration _config;

    public RecordDecisionCommandHandler(ISubmissionDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<Unit> Handle(RecordDecisionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (submission == null)
            throw new KeyNotFoundException("Submission not found.");

        int minReviews = _config.GetValue<int>("SubmissionSettings:MinReviewsRequired");

        if (submission.ReviewsCompletedCount < minReviews && request.Decision != SubmissionStatus.Rejected)
        {
            throw new InvalidOperationException($"Cannot record decision. Requires {minReviews} completed reviews. Current: {submission.ReviewsCompletedCount}");
        }

        submission.Status = request.Decision;

        var auditEntry = new AuditEvent
        {
            ActorId = "EditorInChief",
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