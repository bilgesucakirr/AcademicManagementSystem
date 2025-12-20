using Submission.Domain.Enums;

namespace Submission.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VenueId { get; set; }
    public Guid VenueEditionId { get; set; }
    public Guid CallForPapersId { get; set; }
    public Guid? TrackId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public SubmissionType Type { get; set; }

    public string? ReferenceNumber { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }

    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }
    public bool HasConflictOfInterest { get; set; }
    public string? ConflictDetails { get; set; }

    public Guid SubmitterUserId { get; set; }

    public int ReviewersAssignedCount { get; set; }
    public int ReviewsCompletedCount { get; set; }

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();

    public void Finalize(string referenceNumber)
    {
        if (Status != SubmissionStatus.Draft)
            throw new InvalidOperationException("Only drafts can be finalized.");

        if (string.IsNullOrWhiteSpace(Title)) throw new InvalidOperationException("Title is required.");
        if (!Authors.Any()) throw new InvalidOperationException("At least one author is required.");
        if (!IsOriginal || !HasConsent) throw new InvalidOperationException("Declarations missing.");

        Status = SubmissionStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
    }

    public void UpdateReviewStats(int assignedDelta, int completedDelta)
    {
        ReviewersAssignedCount += assignedDelta;
        ReviewsCompletedCount += completedDelta;
    }
}