using System.Net;
using System.Text.Json;

namespace Venue.Api.Middleware;

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

        var errorResponse = new { success = false, message = exception.Message };

        switch (exception)
        {
            case InvalidOperationException: // Bizim "Only .docx allowed" hatası buraya düşer
                response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                errorResponse = new { success = false, message = "An internal server error occurred." };
                break;
        }

        _logger.LogError(exception, exception.Message);
        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}