using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Entities;

namespace Submission.Application.Features.Submissions.Commands.FinalizeSubmission;

public class FinalizeSubmissionCommandHandler : IRequestHandler<FinalizeSubmissionCommand, string>
{
    private readonly ISubmissionDbContext _context;

    public FinalizeSubmissionCommandHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(FinalizeSubmissionCommand request, CancellationToken ct)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            var submission = await _context.Submissions
                .Include(s => s.Authors)
                .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, ct);

            if (submission == null) throw new KeyNotFoundException("Submission not found");
            if (submission.SubmitterUserId != request.UserId) throw new UnauthorizedAccessException("Not authorized");

            int year = DateTime.UtcNow.Year;

            var counter = await _context.VenueSubmissionCounters
                .FromSqlRaw("SELECT * FROM VenueSubmissionCounters WITH (UPDLOCK, ROWLOCK) WHERE VenueId = {0} AND Year = {1}", submission.VenueId, year)
                .FirstOrDefaultAsync(ct);

            if (counter == null)
            {
                counter = new VenueSubmissionCounter
                {
                    VenueId = submission.VenueId,
                    Year = year,
                    CurrentCount = 0,
                    VenueAcronym = "CONF"
                };
                await _context.VenueSubmissionCounters.AddAsync(counter, ct);
                await _context.SaveChangesAsync(ct);
            }

            counter.CurrentCount++;
            string refNo = $"{counter.VenueAcronym}-{year}-{counter.CurrentCount:D3}";

            submission.Finalize(refNo);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return refNo;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}