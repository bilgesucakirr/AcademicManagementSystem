using MediatR;
using Submission.Application.DTOs;

namespace Submission.Application.Features.Submissions.Queries.GetSubmissionsList;

public record GetSubmissionsListQuery(string UserRole, int? AssignedTrackId) : IRequest<List<SubmissionListDto>>;