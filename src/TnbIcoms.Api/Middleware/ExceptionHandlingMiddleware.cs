using System.Net;
using System.Security.Claims;
using System.Text.Json;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.ErrorLogs;

namespace TnbIcoms.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);

            try
            {
                var errorLogService = context.RequestServices.GetRequiredService<IErrorLogService>();
                var userIdClaim = context.User.FindFirstValue("userId");
                var userId = int.TryParse(userIdClaim, out var uid) ? uid : (int?)null;
                await errorLogService.LogAsync("Backend", "Error", ex.Message, ex.ToString(), context.Request.Path, userId, context.Request.Headers.UserAgent.ToString());
            }
            catch (Exception logEx)
            {
                // Never let error-logging itself become the reason a request fails harder.
                _logger.LogError(logEx, "Failed to persist error log entry");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ApiResponse<object>.Fail("An unexpected error occurred. Please try again later.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
