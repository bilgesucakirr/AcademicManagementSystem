using MediatR;

namespace Review.Application.Features.Assignments.Commands.InviteReviewer;

public record InviteReviewerCommand : IRequest<Guid>
{
    public Guid SubmissionId { get; set; }
    public Guid ReviewerUserId { get; set; }

    public string ReviewerEmail { get; set; } = string.Empty;
    public string SubmissionTitle { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}