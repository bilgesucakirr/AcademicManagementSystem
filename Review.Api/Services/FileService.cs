using Review.Application.Contracts;

namespace Review.Api.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveReviewFileAsync(IFormFile file)
    {
        var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "reviews");
        if (!Directory.Exists(uploadsFolderPath))
        {
            Directory.CreateDirectory(uploadsFolderPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = _httpContextAccessor.HttpContext.Request;
        var fileUrl = $"{request.Scheme}://{request.Host}/uploads/reviews/{uniqueFileName}";

        return fileUrl;
    }
}