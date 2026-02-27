using Microsoft.AspNetCore.Http.HttpResults;

namespace AdvancedFullProject.API.Filters;

public sealed class CorrelationValidationFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.ContainsKey("X-Correlation-ID"))
        {
            return ValueTask.FromResult<object?>(Results.BadRequest(new { message = "X-Correlation-ID header is required." }));
        }

        return next(context);
    }
}
