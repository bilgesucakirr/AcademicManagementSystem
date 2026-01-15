using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Venue.Api.Services;

public interface IFileService { Task<string> SaveFileAsync(IFormFile file); }

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
        // Sadece .docx 
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".docx") throw new InvalidOperationException("Review forms must be in .docx format.");

        // Dosya yolu
        string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folder = Path.Combine(webRootPath, "uploads", "forms");

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var request = _http.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/uploads/forms/{fileName}";
    }
}