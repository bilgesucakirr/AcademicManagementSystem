namespace Review.Application.DTOs;

public class ReviewAssignmentDto
{
    public Guid AssignmentId { get; set; }
    public Guid SubmissionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueAt { get; set; }
}