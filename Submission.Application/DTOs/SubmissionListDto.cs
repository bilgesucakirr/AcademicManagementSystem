namespace Submission.Application.DTOs;

public class SubmissionListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string SubmitterName { get; set; } = string.Empty;
}