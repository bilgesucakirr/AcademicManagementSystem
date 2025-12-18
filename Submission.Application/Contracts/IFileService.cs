using Microsoft.AspNetCore.Http;

namespace Submission.Application.Contracts;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
}