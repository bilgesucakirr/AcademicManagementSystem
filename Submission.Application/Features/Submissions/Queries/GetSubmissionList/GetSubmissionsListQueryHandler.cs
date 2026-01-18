using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Application.DTOs;

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

        query = query.Where(s => s.OrganizerEmail == request.EditorEmail);

        if (request.VenueId.HasValue && request.VenueId != Guid.Empty)
        {
            query = query.Where(s => s.VenueId == request.VenueId.Value);
        }

        if (request.UserRole == "TrackChair" && request.AssignedTrackId.HasValue)
        {
            query = query.Where(s => s.TrackId == request.AssignedTrackId.Value);
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubmissionListDto
            {
                Id = s.Id,
                ReferenceNumber = s.ReferenceNumber,
                Title = s.Title,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt,
                VenueId = s.VenueId,
                ReviewersAssignedCount = s.ReviewersAssignedCount,
                ReviewsCompletedCount = s.ReviewsCompletedCount
            })
            .ToListAsync(cancellationToken);
    }
}