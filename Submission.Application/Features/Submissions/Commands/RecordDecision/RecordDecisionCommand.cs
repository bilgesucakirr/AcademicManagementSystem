using MediatR;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.RecordDecision;

public record RecordDecisionCommand : IRequest<Unit>
{
    public Guid SubmissionId { get; set; }
    public SubmissionStatus Decision { get; set; } // Accepted, Rejected, RevisionRequested
    public string DecisionLetter { get; set; } = string.Empty;
    public bool NotifyAuthor { get; set; } = true;
}