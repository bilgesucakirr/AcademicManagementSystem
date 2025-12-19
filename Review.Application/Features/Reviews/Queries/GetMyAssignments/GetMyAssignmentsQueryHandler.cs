using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;
using Review.Application.DTOs;

namespace Review.Application.Features.Reviews.Queries.GetMyAssignments;

public class GetMyAssignmentsQueryHandler : IRequestHandler<GetMyAssignmentsQuery, List<ReviewAssignmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyAssignmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewAssignmentDto>> Handle(GetMyAssignmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ReviewAssignments
            .Where(a => a.ReviewerUserId == request.ReviewerId && a.Status != "Declined")
            .Select(a => new ReviewAssignmentDto
            {
                AssignmentId = a.Id,
                SubmissionId = a.SubmissionId,
                Status = a.Status,
                DueAt = a.DueAt
            })
            .ToListAsync(cancellationToken);
    }
}