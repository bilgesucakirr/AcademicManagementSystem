using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Application.DTOs;

namespace Submission.Application.Features.Submissions.Queries.GetMySubmissions;

public class GetMySubmissionsQueryHandler : IRequestHandler<GetMySubmissionsQuery, List<SubmissionListDto>>
{
    private readonly ISubmissionDbContext _context;

    public GetMySubmissionsQueryHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubmissionListDto>> Handle(GetMySubmissionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Submissions
            .Where(s => s.SubmitterUserId == request.UserId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubmissionListDto
            {
                Id = s.Id,
                Title = s.Title,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt,
                VenueId = s.VenueId 
            })
            .ToListAsync(cancellationToken);
    }
}