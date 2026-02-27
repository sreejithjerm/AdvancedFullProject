using System.Text;
using AdvancedFullProject.Application.Abstractions.Caching;
using AdvancedFullProject.Application.Abstractions.Messaging;
using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Application.Abstractions.Security;
using AdvancedFullProject.Application.Abstractions.Webhook;
using AdvancedFullProject.Infrastructure.Background;
using AdvancedFullProject.Infrastructure.Caching;
using AdvancedFullProject.Infrastructure.Options;
using AdvancedFullProject.Infrastructure.Persistence.Contexts;
using AdvancedFullProject.Infrastructure.Persistence.Repositories;
using AdvancedFullProject.Infrastructure.Security;
using AdvancedFullProject.Infrastructure.Webhook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;

namespace AdvancedFullProject.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddSingleton<SqlConnectionFactory>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton<IRabbitMqPublisher, Messaging.RabbitMqPublisher>();
        services.AddHttpClient<IWebhookSender, WebhookSender>();

        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(o => o.Configuration = configuration.GetConnectionString("Redis"));

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanReadEmployees", p => p.RequireRole("Admin", "Manager"));
            options.AddPolicy("HighPrivilege", p => p.RequireClaim("role", "Admin"));
        });

        services.AddQuartz(q =>
        {
            var key = new JobKey("employee-sync");
            q.AddJob<EmployeeSyncJob>(o => o.WithIdentity(key));
            q.AddTrigger(t => t.ForJob(key).WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever()));
        });
        services.AddQuartzHostedService();
        services.AddHostedService<HeartbeatHostedService>();

        return services;
    }
}
