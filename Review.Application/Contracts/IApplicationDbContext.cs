using Microsoft.EntityFrameworkCore;
using Review.Domain.Entities;
using System.Collections.Generic;

namespace Review.Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<ReviewAssignment> ReviewAssignments { get; }

    DbSet<Domain.Entities.Review> Reviews { get; }

    // Değişiklikleri kaydetmek için gerekli metodu da sözleşmeye ekliyoruz.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}