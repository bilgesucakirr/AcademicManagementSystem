using Microsoft.AspNetCore.Http;

namespace Review.Application.Contracts;

public interface IFileService
{
    Task<string> SaveReviewFileAsync(IFormFile file);
}