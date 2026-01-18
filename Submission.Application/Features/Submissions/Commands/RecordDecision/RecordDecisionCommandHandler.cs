using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Enums;
using Submission.Domain.Entities;

namespace Submission.Application.Features.Submissions.Commands.RecordDecision;

public class RecordDecisionCommandHandler : IRequestHandler<RecordDecisionCommand, Unit>
{
    private readonly ISubmissionDbContext _context;
    private readonly IEmailService _emailService;

    public RecordDecisionCommandHandler(ISubmissionDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(RecordDecisionCommand request, CancellationToken cancellationToken)
    {
        var sub = await _context.Submissions
            .Include(s => s.Authors)
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (sub == null) throw new KeyNotFoundException("Submission not found.");

        var author = sub.Authors.FirstOrDefault(a => a.IsCorresponding) ?? sub.Authors.First();

        if (request.Decision == SubmissionStatus.Accepted)
        {
            sub.Status = SubmissionStatus.CameraReadyRequested;
            await _emailService.SendDecisionEmailAsync(author.Email, author.FirstName, sub.Title, "Accepted (Camera Ready Required)", request.DecisionLetter);
        }
        else if (request.Decision == SubmissionStatus.Rejected)
        {
            sub.Status = SubmissionStatus.Rejected;
            await _emailService.SendDecisionEmailAsync(author.Email, author.FirstName, sub.Title, "Rejected", request.DecisionLetter);
        }
        else
        {
            sub.Status = request.Decision;
        }

        var auditEntry = new AuditEvent
        {
            ActorId = "EditorInChief",
            Action = "FinalDecisionRecorded",
            EntityType = "Submission",
            EntityId = sub.Id,
            Metadata = $"Final Decision: {request.Decision}. Letter: {request.DecisionLetter}"
        };
        _context.AuditEvents.Add(auditEntry);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}