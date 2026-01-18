using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;
using Review.Domain.Enums;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly ISubmissionIntegrationService _integrationService;

    public SubmitReviewCommandHandler(IApplicationDbContext context, IFileService fileService, ISubmissionIntegrationService integrationService)
    {
        _context = context;
        _fileService = fileService;
        _integrationService = integrationService;
    }

    public async Task<Unit> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(ra => ra.Id == request.AssignmentId, cancellationToken);

        if (assignment == null || assignment.Status != ReviewAssignmentStatus.Accepted)
            throw new InvalidOperationException("Invalid assignment state.");

        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.AssignmentId == request.AssignmentId, cancellationToken);

        if (existingReview != null)
        {
            _context.Reviews.Remove(existingReview);
        }

        string? attachmentUrl = null;
        if (request.ReviewFile != null)
        {
            attachmentUrl = await _fileService.SaveReviewFileAsync(request.ReviewFile);
        }

        var review = new Domain.Entities.Review(
            request.AssignmentId,
            request.OverallScore,
            request.Confidence,
            request.CommentsToAuthor,
            request.CommentsToEditor,
            attachmentUrl,
            request.Recommendation
        );

        assignment.MarkAsSubmitted();
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _integrationService.UpdateStatsAsync(assignment.SubmissionId, 0, 1);
        await _integrationService.NotifyRecommendationAsync(assignment.SubmissionId, request.Recommendation);

        return Unit.Value;
    }
}