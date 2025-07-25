using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Services;
using OperationPrime.Domain.Interfaces;
using OperationPrime.Infrastructure.Data;
using OperationPrime.Infrastructure.Data.Repositories;
using OperationPrime.Infrastructure.Services;

namespace OperationPrime.Infrastructure;

/// <summary>
/// Service registration extensions for the Infrastructure layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddDbContext<OperationPrimeDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IPreIncidentRepository, PreIncidentRepository>();
        services.AddScoped<IMajorIncidentRepository, MajorIncidentRepository>();

        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IPriorityService, PriorityService>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddHttpClient<ITokenProvider, TokenProvider>();
        services.AddHttpClient<INeuronsService, NeuronsService>();
        return services;
    }
}
