using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Services;
using OperationPrime.Domain.Interfaces;
using OperationPrime.Infrastructure.Data;
using OperationPrime.Infrastructure.Data.Repositories;
using OperationPrime.Infrastructure.Services;
using OperationPrime.Infrastructure.Options;

namespace OperationPrime.Infrastructure;

/// <summary>
/// Service registration extensions for the Infrastructure layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILoggerFactory, LoggerFactory>();

        services.Configure<Options.DatabaseOptions>(configuration.GetSection("Database"));

        services.AddDbContext<OperationPrimeDbContext>((sp, options) =>
        {
            var dbOpts = sp.GetRequiredService<IOptions<Options.DatabaseOptions>>().Value;
            var connection = $"{dbOpts.ConnectionString};Password={dbOpts.EncryptionKey}";
            options.UseSqlite(connection);
        });

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IPreIncidentRepository, PreIncidentRepository>();
        services.AddScoped<IMajorIncidentRepository, MajorIncidentRepository>();

        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IPriorityService, PriorityService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddHttpClient<ITokenProvider, TokenProvider>();
        services.AddHttpClient<INeuronsService, NeuronsService>();
        return services;
    }
}
