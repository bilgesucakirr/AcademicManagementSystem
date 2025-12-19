namespace Review.Domain.Entities;

public class Review
{
    public int Id { get; private set; }
    public Guid AssignmentId { get; private set; }

    public decimal OverallScore { get; private set; }
    public int Confidence { get; private set; }

    public string CommentsToAuthor { get; private set; }
    public string? CommentsToEditor { get; private set; }
    public string? AttachmentUrl { get; private set; }

    // YENİ: Editör Kararı Önerisi
    public string Recommendation { get; private set; } // Accept, MinorRevision, MajorRevision, Reject

    public DateTime SubmittedAt { get; private set; }

    public ReviewAssignment Assignment { get; private set; } = null!;

    private Review() { }

    public Review(
        Guid assignmentId,
        decimal overallScore,
        int confidence,
        string commentsToAuthor,
        string? commentsToEditor,
        string? attachmentUrl,
        string recommendation)
    {
        AssignmentId = assignmentId;
        OverallScore = overallScore;
        Confidence = confidence;
        CommentsToAuthor = commentsToAuthor;
        CommentsToEditor = commentsToEditor;
        AttachmentUrl = attachmentUrl;
        Recommendation = recommendation;
        SubmittedAt = DateTime.UtcNow;
    }
}