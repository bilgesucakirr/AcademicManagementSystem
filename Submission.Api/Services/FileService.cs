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
        // DÜZELTME BURADA:
        // Eğer WebRootPath null ise (klasör yoksa), ContentRootPath (proje ana dizini) + "wwwroot" kullan.
        string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

        var folder = Path.Combine(webRootPath, "uploads", "submissions");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var request = _http.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/uploads/submissions/{fileName}";
    }
}