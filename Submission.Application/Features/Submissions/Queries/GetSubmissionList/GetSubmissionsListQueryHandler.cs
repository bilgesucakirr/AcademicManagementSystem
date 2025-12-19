using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Application.DTOs;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Queries.GetSubmissionsList;

public class GetSubmissionsListQueryHandler : IRequestHandler<GetSubmissionsListQuery, List<SubmissionListDto>>
{
    private readonly ISubmissionDbContext _context;

    public GetSubmissionsListQueryHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubmissionListDto>> Handle(GetSubmissionsListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Submissions.AsQueryable();

        if (request.UserRole == "TrackChair")
        {
            if (request.AssignedTrackId.HasValue)
            {
                query = query.Where(s => s.TrackId == request.AssignedTrackId.Value);
            }
            else
            {
                return new List<SubmissionListDto>();
            }
        }
        else if (request.UserRole == "EditorInChief" || request.UserRole == "Admin")
        {

        }
        else
        {
            return new List<SubmissionListDto>();
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubmissionListDto
            {
                Id = s.Id,
                Title = s.Title,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt,
                SubmitterName = "Unknown"
            })
            .ToListAsync(cancellationToken);
    }
}