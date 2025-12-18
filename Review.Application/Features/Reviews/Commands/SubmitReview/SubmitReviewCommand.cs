using MediatR;
using Microsoft.AspNetCore.Http;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public record SubmitReviewCommand : IRequest<Unit>
{
    public Guid AssignmentId { get; set; }
    public decimal OverallScore { get; set; }
    public int Confidence { get; set; }
    public string CommentsToAuthor { get; set; } = string.Empty;
    public string? CommentsToEditor { get; set; }
    public IFormFile? ReviewFile { get; set; }
}