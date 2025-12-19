using MediatR;
using Submission.Application.DTOs;

namespace Submission.Application.Features.Submissions.Queries.GetMySubmissions;

public record GetMySubmissionsQuery(Guid UserId) : IRequest<List<SubmissionListDto>>;