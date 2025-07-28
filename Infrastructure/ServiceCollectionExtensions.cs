using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Application.Interfaces;
using OperationPrime.Infrastructure.Data;
using OperationPrime.Infrastructure.Services;

namespace OperationPrime.Infrastructure;

/// <summary>
/// Extension methods for configuring infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure services with the dependency injection container.
    /// Follows Microsoft DI guidelines for service lifetimes and registration patterns.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register navigation service as singleton (shared across app)
        services.AddSingleton<INavigationService, NavigationService>();

        // Register DbContext as scoped (per unit-of-work)
        services.AddDbContext<OperationPrimeDbContext>(options =>
            options.UseSqlite("Data Source=incidents.db"));

        // Register application services as scoped (per request/operation)
        services.AddScoped<IIncidentService, IncidentService>();

        return services;
    }
}
