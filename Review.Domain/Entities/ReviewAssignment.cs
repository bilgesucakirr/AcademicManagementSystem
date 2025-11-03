namespace Review.Domain.Entities;

public class ReviewAssignment
{
    public Guid Id { get; private set; }
    public int SubmissionId { get; private set; }
    public Guid ReviewerUserId { get; private set; }
    public DateTime InvitedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public string Status { get; private set; } //"Pending", "Accepted", "Declined", "Submitted"

    public Review? Review { get; private set; }

    private ReviewAssignment() { }

    public ReviewAssignment(int submissionId, Guid reviewerUserId, DateTime dueAt)
    {
        Id = Guid.NewGuid();
        SubmissionId = submissionId;
        ReviewerUserId = reviewerUserId;
        DueAt = dueAt;
        InvitedAt = DateTime.UtcNow;
        Status = "Pending";
    }

    public void Accept()
    {
        if (Status != "Pending")
        {
            throw new InvalidOperationException("Only pending assignments can be accepted.");
        }
        Status = "Accepted";
        AcceptedAt = DateTime.UtcNow;
    }

    public void MarkAsSubmitted()
    {
        if (Status != "Accepted")
        {
            throw new InvalidOperationException("Cannot submit a review for an assignment that was not accepted.");
        }
        Status = "Submitted";
    }
}