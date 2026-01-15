using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts;

namespace Review.Application.Features.Assignments.Queries.GetAssignment;

public class ReviewAssignmentDetailDto
{
    public Guid AssignmentId { get; set; }
    public Guid SubmissionId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public record GetAssignmentQuery(Guid AssignmentId) : IRequest<ReviewAssignmentDetailDto?>;

public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, ReviewAssignmentDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAssignmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewAssignmentDetailDto?> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ReviewAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.AssignmentId, cancellationToken);

        if (assignment == null) return null;

        return new ReviewAssignmentDetailDto
        {
            AssignmentId = assignment.Id,
            SubmissionId = assignment.SubmissionId,
          
            Status = assignment.Status.ToString()
        };
    }
}