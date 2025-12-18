using Submission.Application.Contracts;

namespace Submission.Api.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _http;

    public FileService(IWebHostEnvironment env, IHttpContextAccessor http)
    {
        _env = env;
        _http = http;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads", "submissions");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var request = _http.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/uploads/submissions/{fileName}";
    }
}