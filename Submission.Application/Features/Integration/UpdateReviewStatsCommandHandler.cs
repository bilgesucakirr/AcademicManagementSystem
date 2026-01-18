using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;

namespace Submission.Application.Features.Integration;

public class UpdateReviewStatsCommandHandler : IRequestHandler<UpdateReviewStatsCommand, Unit>
{
    private readonly ISubmissionDbContext _context;

    public UpdateReviewStatsCommandHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateReviewStatsCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (submission != null)
        {
            submission.ReviewersAssignedCount += request.AssignedDelta;
            submission.ReviewsCompletedCount += request.CompletedDelta;

            await _context.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}