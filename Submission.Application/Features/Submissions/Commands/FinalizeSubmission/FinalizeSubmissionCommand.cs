using MediatR;

namespace Submission.Application.Features.Submissions.Commands.FinalizeSubmission;

public record FinalizeSubmissionCommand(Guid SubmissionId, Guid UserId) : IRequest<string>;