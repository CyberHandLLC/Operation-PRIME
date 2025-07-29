using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Infrastructure.Data;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Service for managing applications used in incident reporting.
/// Implements auto-suggestion functionality for Applications Affected field.
/// </summary>
public class ApplicationService : IApplicationService
{
    private readonly IDbContextFactory<OperationPrimeDbContext> _contextFactory;
    private readonly ILogger<ApplicationService> _logger;

    /// <summary>
    /// Initializes a new instance of the ApplicationService.
    /// </summary>
    /// <param name="contextFactory">Factory for creating DbContext instances (thread-safe).</param>
    /// <param name="logger">Logger for structured logging.</param>
    public ApplicationService(IDbContextFactory<OperationPrimeDbContext> contextFactory, ILogger<ApplicationService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active applications for auto-suggestion.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of active applications.</returns>
    public async Task<IEnumerable<ApplicationInfo>> GetActiveApplicationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving all active applications from database");
            
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            var applications = await context.Applications
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
                
            _logger.LogInformation("Retrieved {ApplicationCount} active applications from database", applications.Count);
            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve active applications from database");
            throw;
        }
    }

    /// <summary>
    /// Searches applications by name for auto-suggestion.
    /// </summary>
    /// <param name="searchTerm">The search term to filter applications.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of matching applications.</returns>
    public async Task<IEnumerable<ApplicationInfo>> SearchApplicationsAsync(string searchTerm, int maxResults = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetActiveApplicationsAsync(cancellationToken);
            }

            _logger.LogDebug("Searching applications with term: {SearchTerm}", searchTerm);
            
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            var applications = await context.Applications
                .Where(a => a.IsActive && a.Name.Contains(searchTerm))
                .OrderBy(a => a.Name)
                .Take(maxResults)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
                
            _logger.LogInformation("Found {ApplicationCount} applications matching '{SearchTerm}'", applications.Count, searchTerm);
            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search applications with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Creates a new application.
    /// </summary>
    /// <param name="application">The application to create.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created application with assigned ID.</returns>
    public async Task<ApplicationInfo> CreateAsync(ApplicationInfo application, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating new application: {ApplicationName}", application.Name);
            
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            application.CreatedDate = DateTimeOffset.UtcNow;
            context.Applications.Add(application);
            
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            _logger.LogInformation("Successfully created application with ID {ApplicationId}: {ApplicationName}", 
                application.Id, application.Name);
            
            return application;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create application: {ApplicationName}", application.Name);
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with common applications if empty.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task SeedApplicationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            // Check if applications already exist
            var existingCount = await context.Applications.CountAsync(cancellationToken).ConfigureAwait(false);
            if (existingCount > 0)
            {
                _logger.LogDebug("Applications already seeded. Skipping seed operation.");
                return;
            }

            _logger.LogInformation("Seeding applications database with common applications");

            var commonApplications = new[]
            {
                new ApplicationInfo { Name = "Microsoft Outlook", Description = "Email and calendar application" },
                new ApplicationInfo { Name = "Microsoft Teams", Description = "Collaboration and communication platform" },
                new ApplicationInfo { Name = "SharePoint", Description = "Document management and collaboration platform" },
                new ApplicationInfo { Name = "Active Directory", Description = "Directory service for authentication" },
                new ApplicationInfo { Name = "Exchange Server", Description = "Email server and messaging platform" },
                new ApplicationInfo { Name = "SQL Server", Description = "Database management system" },
                new ApplicationInfo { Name = "IIS Web Server", Description = "Web server for hosting applications" },
                new ApplicationInfo { Name = "Office 365", Description = "Cloud-based productivity suite" },
                new ApplicationInfo { Name = "Azure Portal", Description = "Cloud management interface" },
                new ApplicationInfo { Name = "Windows File Server", Description = "File sharing and storage service" },
                new ApplicationInfo { Name = "Print Server", Description = "Centralized printing service" },
                new ApplicationInfo { Name = "VPN Gateway", Description = "Remote access service" },
                new ApplicationInfo { Name = "Domain Controller", Description = "Network authentication service" },
                new ApplicationInfo { Name = "DHCP Server", Description = "IP address assignment service" },
                new ApplicationInfo { Name = "DNS Server", Description = "Domain name resolution service" }
            };

            context.Applications.AddRange(commonApplications);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            _logger.LogInformation("Successfully seeded {ApplicationCount} applications", commonApplications.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed applications database");
            throw;
        }
    }
}
