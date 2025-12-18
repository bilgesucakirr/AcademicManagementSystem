using Submission.Domain.Enums;

namespace Submission.Domain.Entities;

public class SubmissionFile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string OriginalFileName { get; set; } = string.Empty; // user_upload.pdf
    public string StoragePath { get; set; } = string.Empty;      // /uploads/guid.pdf
    public string ContentType { get; set; } = string.Empty;      // application/pdf
    public long Size { get; set; }

    public FileType Type { get; set; }

    // İlişki
    public Guid SubmissionId { get; set; }
}