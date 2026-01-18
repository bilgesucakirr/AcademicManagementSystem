using Review.Application.Contracts;
using System.Net.Http.Json;

namespace Review.Infrastructure.Services;

public class SubmissionIntegrationService : ISubmissionIntegrationService
{
    private readonly HttpClient _httpClient;
    public SubmissionIntegrationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-Internal-Key", "INTERNAL_SECRET_KEY_123");
    }

    public async Task UpdateStatsAsync(Guid submissionId, int assignedDelta, int completedDelta)
    {
        var payload = new { AssignedDelta = assignedDelta, CompletedDelta = completedDelta };
        await _httpClient.PostAsJsonAsync($"api/integration/submissions/{submissionId}/review-stats", payload);
    }

    public async Task NotifyRecommendationAsync(Guid submissionId, string recommendation)
    {
        var payload = new { Recommendation = recommendation };
        await _httpClient.PostAsJsonAsync($"api/integration/submissions/{submissionId}/reviewer-decision", payload);
    }
}