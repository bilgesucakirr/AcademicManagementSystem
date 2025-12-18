using System.Net;
using System.Text.Json;

namespace Review.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new
        {
            success = false,
            message = "Bir hata oluştu." // Varsayılan mesaj
        };

        switch (exception)
        {
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                errorResponse = new { success = false, message = exception.Message };
                break;
            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                errorResponse = new { success = false, message = exception.Message };
                break;
            // Buraya FluentValidation'dan gelen ValidationException gibi başka özel hatalar da ekleyebiliriz.
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                errorResponse = new { success = false, message = "Sunucuda beklenmedik bir hata oluştu!" };
                break;
        }

        _logger.LogError(exception, "Hata Yakalandı: {message}", exception.Message);
        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}