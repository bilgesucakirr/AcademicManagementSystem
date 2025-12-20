namespace Submission.Domain.Entities;

public class VenueSubmissionCounter
{
    public Guid VenueId { get; set; }
    public int Year { get; set; }
    public int CurrentCount { get; set; }
    public string VenueAcronym { get; set; } = string.Empty;
}