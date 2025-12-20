using MediatR;

namespace Submission.Application.Features.Integration;

public record UpdateReviewStatsCommand(Guid SubmissionId, int AssignedDelta, int CompletedDelta) : IRequest<Unit>;