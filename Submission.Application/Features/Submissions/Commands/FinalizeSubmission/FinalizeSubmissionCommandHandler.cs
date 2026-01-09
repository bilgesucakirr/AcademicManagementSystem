using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

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
        // DÜZELTME BURADA: .Include(s => s.Authors) eklendi.
        // Bu sayede makaleyi çekerken içindeki yazarları da "yükle" diyoruz.
        var submission = await _context.Submissions
            .Include(s => s.Authors)
            .FirstOrDefaultAsync(s => s.Id == request.SubmissionId, ct);

        if (submission == null) throw new Exception("Submission not found");

        if (submission.Status != SubmissionStatus.Draft)
            return submission.ReferenceNumber ?? "ALREADY-SUBMITTED";

        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            int year = DateTime.UtcNow.Year;

            var counter = await _context.VenueSubmissionCounters
                .FirstOrDefaultAsync(c => c.VenueId == submission.VenueId && c.Year == year, ct);

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

            while (await _context.Submissions.AnyAsync(s => s.ReferenceNumber == refNo, ct))
            {
                counter.CurrentCount++;
                refNo = $"{counter.VenueAcronym}-{year}-{counter.CurrentCount:D3}";
            }

            // Artık Authors listesi dolu olduğu için burası hata vermeyecek
            submission.Finalize(refNo);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return refNo;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            Console.WriteLine("SUBMISSION FINALIZATION ERROR: " + ex.Message);
            throw;
        }
    }
}