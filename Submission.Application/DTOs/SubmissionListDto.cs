namespace Submission.Application.DTOs;

public class SubmissionListDto
{
    public Guid Id { get; set; }
    public string? ReferenceNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string SubmitterName { get; set; } = string.Empty;
    public Guid VenueId { get; set; }
    public int ReviewersAssignedCount { get; set; }
    public int ReviewsCompletedCount { get; set; }
}