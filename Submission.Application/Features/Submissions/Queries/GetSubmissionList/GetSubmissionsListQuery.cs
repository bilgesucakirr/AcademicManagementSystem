using MediatR;
using Submission.Application.DTOs;

namespace Submission.Application.Features.Submissions.Queries.GetSubmissionsList;

public record GetSubmissionsListQuery(string UserRole, Guid? AssignedTrackId, Guid? VenueId, string EditorEmail) : IRequest<List<SubmissionListDto>>;