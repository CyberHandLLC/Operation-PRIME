using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Services;

namespace OperationPrime.Infrastructure;

/// <summary>
/// Extension methods for configuring infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds simplified infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register the simple in-memory incident service
        services.AddSingleton<IIncidentService, IncidentService>();



        return services;
    }
}
