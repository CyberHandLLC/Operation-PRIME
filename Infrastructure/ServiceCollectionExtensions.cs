using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Application.Interfaces;
using OperationPrime.Infrastructure.Services;

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
        // Register navigation service as singleton (shared across app)
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }
}
