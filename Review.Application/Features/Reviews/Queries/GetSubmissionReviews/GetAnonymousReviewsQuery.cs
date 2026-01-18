using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;

namespace Review.Application.Features.Reviews.Queries.GetSubmissionReviews;

public record GetAnonymousReviewsQuery(Guid SubmissionId) : IRequest<List<AnonymousReviewDto>>;

public class AnonymousReviewDto
{
    public decimal OverallScore { get; set; }
    public string CommentsToAuthor { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class GetAnonymousReviewsQueryHandler : IRequestHandler<GetAnonymousReviewsQuery, List<AnonymousReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAnonymousReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AnonymousReviewDto>> Handle(GetAnonymousReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.Assignment)
            .Where(r => r.Assignment.SubmissionId == request.SubmissionId)
            .Select(r => new AnonymousReviewDto
            {
                OverallScore = r.OverallScore,
                CommentsToAuthor = r.CommentsToAuthor,
                AttachmentUrl = r.AttachmentUrl,
                SubmittedAt = r.SubmittedAt
            })
            .ToListAsync(cancellationToken);
    }
}