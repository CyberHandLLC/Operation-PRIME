using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Configuration;
using OperationPrime.Infrastructure.Data;
using OperationPrime.Infrastructure.Services;
using OperationPrime.Application.Services;

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
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration settings
        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        services.Configure<SeedDataSettings>(configuration.GetSection("SeedData"));
        
        // Register navigation service as scoped (follows WinUI 3 best practices for MVVM lifecycle)
        services.AddScoped<INavigationService, NavigationService>();

        // Register DbContextFactory for thread-safe DbContext creation (Microsoft best practice)
        // Factory pattern enables proper disposal and thread safety in async UI applications
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=incidents.db";
        services.AddDbContextFactory<OperationPrimeDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register application services as scoped (per request/operation)
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IIncidentDataMappingService, IncidentDataMappingService>();
        
        // Register refactored workflow and validation services as scoped
        services.AddScoped<IIncidentWorkflowService, IncidentWorkflowService>();
        services.AddScoped<IIncidentValidationService, IncidentValidationService>();
        
        // Register new Clean Architecture services
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IIncidentOrchestrationService, IncidentOrchestrationService>();
        
        // Register presentation helpers
        services.AddScoped<OperationPrime.Presentation.Helpers.ViewModelCollectionsManager>();

        return services;
    }
}
