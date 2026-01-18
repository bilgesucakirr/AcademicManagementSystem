using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Enums;
using Submission.Domain.Entities;

namespace Submission.Application.Features.Integration;

public record ProcessReviewerDecisionCommand(Guid SubmissionId, string Recommendation) : IRequest<Unit>;

public class ProcessReviewerDecisionCommandHandler : IRequestHandler<ProcessReviewerDecisionCommand, Unit>
{
    private readonly ISubmissionDbContext _context;
    private readonly IEmailService _emailService;

    public ProcessReviewerDecisionCommandHandler(ISubmissionDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ProcessReviewerDecisionCommand request, CancellationToken cancellationToken)
    {
        var sub = await _context.Submissions.Include(x => x.Authors).FirstOrDefaultAsync(x => x.Id == request.SubmissionId);
        if (sub == null) return Unit.Value;

        if (request.Recommendation.Contains("Revision"))
        {
            sub.Status = request.Recommendation == "MinorRevision" ? SubmissionStatus.MinorRevisionRequired : SubmissionStatus.MajorRevisionRequired;
            var author = sub.Authors.FirstOrDefault(a => a.IsCorresponding) ?? sub.Authors.First();

            await _emailService.SendDecisionEmailAsync(author.Email, author.FirstName, sub.Title, sub.Status.ToString(), "The reviewers have completed their evaluation and requested revisions to your work. Please check your dashboard for details.");
        }
        else if (request.Recommendation == "Accept")
        {
            if (!string.IsNullOrEmpty(sub.OrganizerEmail))
                await _emailService.SendSubmissionReceiptAsync(sub.OrganizerEmail, "Editor", $"Reviewer recommended Acceptance for paper {sub.ReferenceNumber}. Final decision required.");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}