using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;
using Review.Domain.Enums;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public SubmitReviewCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Unit> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(ra => ra.Id == request.AssignmentId, cancellationToken);

        if (assignment is null)
            throw new KeyNotFoundException($"Assignment {request.AssignmentId} not found.");

        if (assignment.Status != ReviewAssignmentStatus.Accepted)
            throw new InvalidOperationException("You can only submit reviews for accepted assignments.");

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

        // Atamayı 'Submitted' (Gönderildi) olarak işaretle
        assignment.MarkAsSubmitted();

        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}