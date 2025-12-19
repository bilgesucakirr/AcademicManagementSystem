using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;

namespace Submission.Application.Contracts;

public interface ISubmissionDbContext
{
    DbSet<Domain.Entities.Submission> Submissions { get; }
    DbSet<Author> Authors { get; }
    DbSet<SubmissionFile> SubmissionFiles { get; }
    DbSet<AuditEvent> AuditEvents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}