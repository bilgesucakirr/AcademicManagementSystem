using Submission.Domain.Enums;

namespace Submission.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- SYSTEM INFO ---
    // Benzersiz Referans No (Örn: CONF-2025-1042) - Submit anında üretilir.
    public string? ReferenceNumber { get; private set; }
    public SubmissionStatus Status { get; private set; } = SubmissionStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; private set; }

    // --- CONTEXT (Konferans/Dergi Ayrımı) ---
    // Bir başvuru YA Konferansa YA Dergiye yapılır.
    public int? ConferenceId { get; set; }
    public int? TrackId { get; set; } // Konu başlığı (CyberSecurity, AI vb.)
    public int? JournalId { get; set; }

    // --- MANUSCRIPT METADATA ---
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    // Keywords array istenmişti, veritabanında virgülle ayrılmış string tutmak pratiktir.
    // DTO'da array'e çeviririz.
    public string Keywords { get; set; } = string.Empty;
    public SubmissionType Type { get; set; }

    // --- ETHICAL DECLARATIONS (Etik Beyanlar) ---
    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; } // Başka yerde incelemede değil
    public bool HasConsent { get; set; }     // Tüm yazarların onayı var
    public bool HasConflictOfInterest { get; set; }
    public string? ConflictDetails { get; set; }

    // --- OWNER ---
    public Guid SubmitterUserId { get; set; } // Identity servisinden gelen ID

    // --- RELATIONS ---
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();

    // --- DOMAIN METHODS (İş Kuralları) ---

    // Taslağı nihai olarak gönderme işlemi
    public void Submit(string referenceNumber)
    {
        // Validasyonlar (Prompt'taki "System Behavior" kısmı)
        if (string.IsNullOrWhiteSpace(Title)) throw new InvalidOperationException("Title is required.");
        if (string.IsNullOrWhiteSpace(Abstract)) throw new InvalidOperationException("Abstract is required.");
        if (!Authors.Any()) throw new InvalidOperationException("At least one author is required.");
        if (!Files.Any(f => f.Type == FileType.MainManuscript)) throw new InvalidOperationException("Main manuscript file is missing.");

        // Etik kurallar onaylanmadıysa gönderilemez
        if (!IsOriginal || !IsNotElsewhere || !HasConsent)
            throw new InvalidOperationException("Ethical declarations must be confirmed.");

        ReferenceNumber = referenceNumber;
        Status = SubmissionStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }
}