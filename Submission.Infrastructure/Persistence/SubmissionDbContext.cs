using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;
using Submission.Application.Contracts;
using System.Reflection;

namespace Submission.Infrastructure.Persistence;

public class SubmissionDbContext : DbContext, ISubmissionDbContext
{
    public SubmissionDbContext(DbContextOptions<SubmissionDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Submission> Submissions { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<SubmissionFile> SubmissionFiles { get; set; }
    public DbSet<AuditEvent> AuditEvents { get; set; }
    public DbSet<VenueSubmissionCounter> VenueSubmissionCounters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VenueSubmissionCounter>()
            .HasKey(c => new { c.VenueId, c.Year });

        modelBuilder.Entity<Domain.Entities.Submission>()
            .HasIndex(s => s.ReferenceNumber)
            .IsUnique()
            .HasFilter("[ReferenceNumber] IS NOT NULL");
    }
}