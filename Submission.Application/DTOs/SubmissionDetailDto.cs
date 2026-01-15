using Submission.Domain.Enums;

namespace Submission.Application.DTOs;

public class SubmissionDetailDto
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<AuthorDto> Authors { get; set; } = new();
    public List<FileDto> Files { get; set; } = new();
}

public class FileDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}