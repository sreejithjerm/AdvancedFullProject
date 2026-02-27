namespace AdvancedFullProject.API.Middlewares;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string HeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        await _next(context);
    }
}
