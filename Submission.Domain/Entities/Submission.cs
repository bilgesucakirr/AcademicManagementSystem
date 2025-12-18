using Submission.Domain.Enums;

namespace Submission.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    
    public SubmissionContext Context { get; set; }

    public string? ReferenceNumber { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }

    public int? ConferenceId { get; set; }
    public int? TrackId { get; set; }
    public int? JournalId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public SubmissionType Type { get; set; }

    // Etik Beyanlar
    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }
    public bool HasConflictOfInterest { get; set; }
    public string? ConflictDetails { get; set; }

    // --- OWNER ---
    public Guid SubmitterUserId { get; set; }

    // --- RELATIONS ---
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();

  
    public void Submit(string referenceNumber)
    {
     
        if (string.IsNullOrWhiteSpace(Title)) throw new InvalidOperationException("Title is required.");
        if (string.IsNullOrWhiteSpace(Abstract)) throw new InvalidOperationException("Abstract is required.");
        if (!Authors.Any()) throw new InvalidOperationException("At least one author is required.");

        
        if (!IsOriginal || !IsNotElsewhere || !HasConsent)
            throw new InvalidOperationException("Ethical declarations must be confirmed.");

        ReferenceNumber = referenceNumber;
        Status = SubmissionStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }
}