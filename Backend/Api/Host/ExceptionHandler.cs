using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace ModernTenon.Api.Host;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case KeyNotFoundException keyNotFoundException:
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = keyNotFoundException.Message
                }, cancellationToken);
                break;
            default:
                _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "Internal Server Error"
                }, cancellationToken);
                break;
        }

        return true;
    }
}
