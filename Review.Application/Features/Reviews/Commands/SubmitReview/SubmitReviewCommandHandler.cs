using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts; 

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public SubmitReviewCommandHandler(IApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<Unit> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(ra => ra.Id == request.AssignmentId, cancellationToken);

        if (assignment is null)
        {
            throw new KeyNotFoundException($"Assignment with ID {request.AssignmentId} not found.");
        }

        if (assignment.Status != "Accepted")
        {
            throw new InvalidOperationException("Cannot submit a review for an assignment that is not in 'Accepted' status.");
        }

        var newReview = new Domain.Entities.Review(
            request.AssignmentId,
            request.OverallScore,
            request.Confidence,
            request.CommentsToAuthor,
            request.CommentsToEditor
        );

        assignment.MarkAsSubmitted();

        await _context.Reviews.AddAsync(newReview, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}