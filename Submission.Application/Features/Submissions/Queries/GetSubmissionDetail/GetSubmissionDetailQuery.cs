using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Application.DTOs;

namespace Submission.Application.Features.Submissions.Queries.GetSubmissionDetail;

public record GetSubmissionDetailQuery(Guid Id) : IRequest<SubmissionDetailDto?>;

public class GetSubmissionDetailQueryHandler : IRequestHandler<GetSubmissionDetailQuery, SubmissionDetailDto?>
{
    private readonly ISubmissionDbContext _context;

    public GetSubmissionDetailQueryHandler(ISubmissionDbContext context)
    {
        _context = context;
    }

    public async Task<SubmissionDetailDto?> Handle(GetSubmissionDetailQuery request, CancellationToken cancellationToken)
    {
        var s = await _context.Submissions
            .Include(x => x.Authors)
            .Include(x => x.Files)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (s == null) return null;

        return new SubmissionDetailDto
        {
            Id = s.Id,
            VenueId = s.VenueId,
            ReferenceNumber = s.ReferenceNumber ?? "DRAFT",
            Title = s.Title,
            Abstract = s.Abstract,
            Keywords = s.Keywords,
            Status = s.Status.ToString(),
            CreatedAt = s.CreatedAt,
            Authors = s.Authors.Select(a => new AuthorDto
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Affiliation = a.Affiliation,
                Country = a.Country,
                IsCorresponding = a.IsCorresponding
            }).ToList(),
            Files = s.Files.Select(f => new FileDto
            {
                FileName = f.OriginalFileName,
                FileUrl = f.StoragePath,
                Type = f.Type.ToString()
            }).ToList()
        };
    }
}