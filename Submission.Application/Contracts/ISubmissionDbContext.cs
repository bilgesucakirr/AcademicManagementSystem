using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; 
using Submission.Domain.Entities;

namespace Submission.Application.Contracts;

public interface ISubmissionDbContext
{
    DbSet<Domain.Entities.Submission> Submissions { get; }
    DbSet<Author> Authors { get; }
    DbSet<SubmissionFile> SubmissionFiles { get; }
    DbSet<AuditEvent> AuditEvents { get; }

   
    DbSet<VenueSubmissionCounter> VenueSubmissionCounters { get; }
    DatabaseFacade Database { get; } // Transaction açabilmek için gerekli

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}