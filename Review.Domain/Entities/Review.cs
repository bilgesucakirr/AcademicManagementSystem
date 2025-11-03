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
    public DateTime SubmittedAt { get; private set; }

    public ReviewAssignment Assignment { get; private set; } = null!;

    private Review() { }

    public Review(
        Guid assignmentId,
        decimal overallScore,
        int confidence,
        string commentsToAuthor,
        string? commentsToEditor)
    {
        
        if (overallScore < 1 || overallScore > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(overallScore), "Score must be between 1 and 5.");
        }

        AssignmentId = assignmentId;
        OverallScore = overallScore;
        Confidence = confidence;
        CommentsToAuthor = commentsToAuthor;
        CommentsToEditor = commentsToEditor;
        SubmittedAt = DateTime.UtcNow;
    }
}