namespace Review.Application.Contracts;

public interface ISubmissionIntegrationService
{
    Task UpdateStatsAsync(Guid submissionId, int assignedDelta, int completedDelta);
}