using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;
using System.Collections.Generic;

namespace Submission.Application.Contracts;

public interface ISubmissionDbContext
{
    DbSet<Domain.Entities.Submission> Submissions { get; }
    DbSet<Author> Authors { get; }
    DbSet<SubmissionFile> SubmissionFiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}