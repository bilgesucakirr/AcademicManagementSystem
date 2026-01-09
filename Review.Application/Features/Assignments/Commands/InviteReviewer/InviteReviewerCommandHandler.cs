using MediatR;
using Review.Application.Contracts;
using Review.Domain.Entities;
using Review.Domain.Enums;

namespace Review.Application.Features.Assignments.Commands.InviteReviewer;

public class InviteReviewerCommandHandler : IRequestHandler<InviteReviewerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public InviteReviewerCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(InviteReviewerCommand request, CancellationToken cancellationToken)
    {
        var existingAssignment = _context.ReviewAssignments
            .FirstOrDefault(ra =>
                ra.SubmissionId == request.SubmissionId &&
                ra.ReviewerUserId == request.ReviewerUserId &&
                ra.Status != ReviewAssignmentStatus.Declined);

        if (existingAssignment != null)
        {
            throw new InvalidOperationException("This reviewer has already been invited.");
        }

        var dueAt = request.DueDate ?? DateTime.UtcNow.AddDays(14);

        var assignment = ReviewAssignment.CreateInvitation(
            request.SubmissionId,
            request.ReviewerUserId,
            dueAt
        );

        await _context.ReviewAssignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendInvitationEmailAsync(request.ReviewerEmail, assignment.Id.ToString(), request.SubmissionTitle);

        return assignment.Id;
    }
}