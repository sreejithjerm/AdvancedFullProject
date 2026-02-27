using System.IO.Compression;
using Asp.Versioning;
using AdvancedFullProject.API.Extensions;
using AdvancedFullProject.API.Filters;
using AdvancedFullProject.API.Middlewares;
using AdvancedFullProject.Application.DependencyInjection;
using AdvancedFullProject.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiLayer(builder.Configuration);

builder.Services.AddResponseCaching();
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("fixed", o =>
{
    o.PermitLimit = 100;
    o.Window = TimeSpan.FromMinutes(1);
}));
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseExceptionHandler();
app.UseResponseCompression();
app.UseRateLimiter();
app.UseResponseCaching();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.MapGet("/api/v{version:apiVersion}/minimal/ping", () => Results.Ok(new { message = "pong" }))
   .WithApiVersionSet(ApiVersionSetFactory.Build(app))
   .MapToApiVersion(new ApiVersion(1, 0))
   .AddEndpointFilter<CorrelationValidationFilter>();

app.Run();

public partial class Program;
