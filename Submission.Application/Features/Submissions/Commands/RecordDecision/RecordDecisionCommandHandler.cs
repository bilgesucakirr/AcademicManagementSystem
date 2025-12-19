using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.RecordDecision;

public class RecordDecisionCommandHandler : IRequestHandler<RecordDecisionCommand, Unit>
{
    private readonly ISubmissionDbContext _context;

    public RecordDecisionCommandHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RecordDecisionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, cancellationToken);

        if (submission == null)
            throw new KeyNotFoundException("Submission not found.");

        // Karara göre statüyü güncelle
        submission.Status = request.Decision;

        // Eğer kabul edildiyse veya reddedildiyse SubmittedAt gibi tarihleri de güncelleyebiliriz
        // JARVIS Kuralı: Karar verildiğinde sistem bir Audit Log oluşturmalı (Şimdilik Console'a yazalım)
        Console.WriteLine($"[DECISION RECORDED] Submission: {submission.Id}, Decision: {request.Decision}");
        Console.WriteLine($"[LETTER]: {request.DecisionLetter}");

        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Notification Outbox'a bir kayıt atılmalı (Master Prompt Madde H)

        return Unit.Value;
    }
}