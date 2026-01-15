using Microsoft.AspNetCore.Http; // IFormFile ve IHttpContextAccessor için gerekli
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için gerekli

namespace Identity.Api.Services;

public interface IFileService
{
    Task<string> SaveProfilePictureAsync(IFormFile file);
}

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _http;

    public FileService(IWebHostEnvironment env, IHttpContextAccessor http)
    {
        _env = env;
        _http = http;
    }

    public async Task<string> SaveProfilePictureAsync(IFormFile file)
    {
        // 1. Format Kontrolü
        var ext = Path.GetExtension(file.FileName).ToLower();
        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Only .jpg, .jpeg and .png formats are allowed.");

        // 2. Boyut Kontrolü (2MB)
        if (file.Length > 2 * 1024 * 1024)
            throw new InvalidOperationException("File size cannot exceed 2MB.");

        // WebRootPath null gelirse manuel oluştur
        string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folder = Path.Combine(webRootPath, "uploads", "avatars");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var request = _http.HttpContext!.Request;
        // Tam URL döndür
        return $"{request.Scheme}://{request.Host}/uploads/avatars/{fileName}";
    }
}