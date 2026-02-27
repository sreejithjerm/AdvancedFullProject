using Serilog;

namespace AdvancedFullProject.API.Middlewares;

public sealed class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        Log.Information("Incoming request {Method} {Path}", context.Request.Method, context.Request.Path);
        await _next(context);
        Log.Information("Outgoing response {StatusCode}", context.Response.StatusCode);
    }
}
