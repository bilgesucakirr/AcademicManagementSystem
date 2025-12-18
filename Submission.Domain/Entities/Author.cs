namespace Submission.Domain.Entities;

public class Author
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Affiliation { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Orcid { get; set; }

    public bool IsCorresponding { get; set; }
    public int Order { get; set; }

    // İlişki
    public Guid SubmissionId { get; set; }
}