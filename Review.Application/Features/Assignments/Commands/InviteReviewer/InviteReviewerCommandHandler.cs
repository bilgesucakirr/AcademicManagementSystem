using MediatR;
using Review.Application.Contracts;
using Review.Domain.Entities;
using Review.Domain.Enums;

namespace Review.Application.Features.Assignments.Commands.InviteReviewer;

public class InviteReviewerCommandHandler : IRequestHandler<InviteReviewerCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public InviteReviewerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(InviteReviewerCommand request, CancellationToken cancellationToken)
    {
        // 1. Bu kullanıcıya zaten bu makale için atama yapılmış mı?
        // (Reddedilmiş veya süresi dolmuş olsa bile aynı kişiyi tekrar davet etmeyi engelleyebiliriz veya izin verebiliriz. Şimdilik açık bir atama var mı ona bakalım.)
        var existingAssignment = _context.ReviewAssignments
            .FirstOrDefault(ra =>
                ra.SubmissionId == request.SubmissionId &&
                ra.ReviewerUserId == request.ReviewerUserId &&
                ra.Status != ReviewAssignmentStatus.Declined); // Reddeden tekrar davet edilebilir mantığı

        if (existingAssignment != null)
        {
            throw new InvalidOperationException("This reviewer has already been invited or assigned to this submission.");
        }

        // 2. Default süre 14 gün
        var dueAt = request.DueDate ?? DateTime.UtcNow.AddDays(14);

        // 3. Domain Entity'yi Factory Method ile oluştur
        var assignment = ReviewAssignment.CreateInvitation(
            request.SubmissionId,
            request.ReviewerUserId,
            dueAt
        );

        // 4. Veritabanına ekle
        await _context.ReviewAssignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Burada bir "EmailService" tetiklenip hakeme mail atılmalı.
        // "Sayın X, Y başlıklı makaleyi değerlendirmeniz için davet edildiniz."

        return assignment.Id;
    }
}