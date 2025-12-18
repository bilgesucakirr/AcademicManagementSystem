using Microsoft.EntityFrameworkCore;
using Review.Domain.Entities;

namespace Review.Infrastructure.Persistence;

public static class SeedData
{
    public static readonly Guid TestAssignmentId = new("11111111-2222-3333-4444-555555555999");

    public static async Task InitializeDatabaseAsync(ReviewDbContext context)
    {
        await context.Database.MigrateAsync();

        
        if (await context.ReviewAssignments.AnyAsync())
        {
            return; 
        }

      
        var testAssignment = new ReviewAssignment(
            submissionId: 101,
            reviewerUserId: Guid.NewGuid(),
            dueAt: DateTime.UtcNow.AddDays(10)
        );

        
        var idProperty = typeof(ReviewAssignment).GetProperty(nameof(ReviewAssignment.Id));
        idProperty!.SetValue(testAssignment, TestAssignmentId);

       
        testAssignment.Accept();

        await context.ReviewAssignments.AddAsync(testAssignment);
        await context.SaveChangesAsync();

        Console.WriteLine("--> Veritabanı test verisi ile dolduruldu.");
    }
}