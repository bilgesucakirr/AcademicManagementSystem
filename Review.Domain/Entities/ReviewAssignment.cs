using Review.Domain.Enums;

namespace Review.Domain.Entities;

public class ReviewAssignment
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Guid ReviewerUserId { get; private set; }

    public ReviewAssignmentStatus Status { get; private set; }

    // Davet ve Süreç Tarihleri
    public DateTime InvitedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }

    public string? DeclineReason { get; private set; } // Reddiyse neden?

    // Navigation Property
    public Review? Review { get; private set; }

    private ReviewAssignment() { } // EF Core için

    // Factory Method: Yeni bir davet oluşturur (Henüz kabul edilmedi)
    public static ReviewAssignment CreateInvitation(Guid submissionId, Guid reviewerUserId, DateTime dueAt)
    {
        return new ReviewAssignment
        {
            Id = Guid.NewGuid(),
            SubmissionId = submissionId,
            ReviewerUserId = reviewerUserId,
            DueAt = dueAt,
            InvitedAt = DateTime.UtcNow,
            Status = ReviewAssignmentStatus.Invited
        };
    }

    // Hakem daveti kabul eder
    public void AcceptInvitation()
    {
        if (Status != ReviewAssignmentStatus.Invited)
            throw new InvalidOperationException($"Cannot accept invitation. Current status is {Status}");

        if (DateTime.UtcNow > DueAt)
            throw new InvalidOperationException("Invitation has expired.");

        Status = ReviewAssignmentStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
    }

    // Hakem daveti reddeder
    public void DeclineInvitation(string reason)
    {
        if (Status != ReviewAssignmentStatus.Invited)
            throw new InvalidOperationException($"Cannot decline invitation. Current status is {Status}");

        Status = ReviewAssignmentStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        DeclineReason = reason;
    }

    // Değerlendirme gönderildiğinde çağrılır
    public void MarkAsSubmitted()
    {
        if (Status != ReviewAssignmentStatus.Accepted)
            throw new InvalidOperationException("Cannot submit review. Assignment is not in Accepted state.");

        Status = ReviewAssignmentStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }
}