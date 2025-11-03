using Microsoft.EntityFrameworkCore;
using Review.Domain.Entities;
using System.Reflection;
using Review.Application.Contracts;

namespace Review.Infrastructure.Persistence;

public class ReviewDbContext : DbContext, IApplicationDbContext
{
    
    public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

   
    public DbSet<ReviewAssignment> ReviewAssignments { get; set; }
    public DbSet<Domain.Entities.Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}