using Review.Domain.Enums;

namespace Review.Domain.Entities;

public class ReviewAssignment
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Guid ReviewerUserId { get; private set; }
    public string ReviewerEmail { get; private set; } = string.Empty;
    public ReviewAssignmentStatus Status { get; private set; }
    public DateTime InvitedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public string? DeclineReason { get; private set; }
    public Review? Review { get; private set; }

    private ReviewAssignment() { }

    public static ReviewAssignment CreateInvitation(Guid submissionId, Guid reviewerUserId, string reviewerEmail, DateTime dueAt)
    {
        return new ReviewAssignment
        {
            Id = Guid.NewGuid(),
            SubmissionId = submissionId,
            ReviewerUserId = reviewerUserId,
            ReviewerEmail = reviewerEmail,
            DueAt = dueAt,
            InvitedAt = DateTime.UtcNow,
            Status = ReviewAssignmentStatus.Invited
        };
    }

    public void AcceptInvitation()
    {
        if (Status != ReviewAssignmentStatus.Invited)
            throw new InvalidOperationException($"Cannot accept invitation. Current status is {Status}");

        if (DateTime.UtcNow > DueAt)
            throw new InvalidOperationException("Invitation has expired.");

        Status = ReviewAssignmentStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
    }

    public void DeclineInvitation(string reason)
    {
        if (Status != ReviewAssignmentStatus.Invited)
            throw new InvalidOperationException($"Cannot decline invitation. Current status is {Status}");

        Status = ReviewAssignmentStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        DeclineReason = reason;
    }

    public void MarkAsSubmitted()
    {
        if (Status != ReviewAssignmentStatus.Accepted)
            throw new InvalidOperationException("Cannot submit review. Assignment is not in Accepted state.");

        Status = ReviewAssignmentStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void ReActivate()
    {
        this.Status = ReviewAssignmentStatus.Accepted;
    }
}