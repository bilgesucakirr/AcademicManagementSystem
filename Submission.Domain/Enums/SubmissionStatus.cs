namespace Submission.Domain.Enums;

public enum SubmissionStatus
{
    Draft,              // Yazar hala düzenliyor
    Submitted,          // Gönderildi, Editör bekliyor
    UnderReview,        // Hakemlerde
    RevisionRequired,   // Yazar düzeltme yapmalı
    Accepted,
    Rejected
}