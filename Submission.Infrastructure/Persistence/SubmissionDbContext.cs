using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;
using System.Reflection;
using Submission.Application.Contracts;

namespace Submission.Infrastructure.Persistence;

public class SubmissionDbContext : DbContext, ISubmissionDbContext
{
    public SubmissionDbContext(DbContextOptions<SubmissionDbContext> options) : base(options) { }

    public DbSet<Submission.Domain.Entities.Submission> Submissions { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<SubmissionFile> SubmissionFiles { get; set; }
    public DbSet<AuditEvent> AuditEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}