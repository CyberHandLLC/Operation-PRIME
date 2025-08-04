using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Infrastructure.Data;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Service implementation for incident data operations.
/// Uses Entity Framework Core with SQLite for data persistence.
/// </summary>
public class IncidentService : IIncidentService
{
    private readonly IDbContextFactory<OperationPrimeDbContext> _contextFactory;
    private readonly ILogger<IncidentService> _logger;

    /// <summary>
    /// Initializes a new instance of the IncidentService.
    /// </summary>
    /// <param name="contextFactory">Factory for creating DbContext instances (thread-safe).</param>
    /// <param name="logger">Logger for structured logging.</param>
    public IncidentService(IDbContextFactory<OperationPrimeDbContext> contextFactory, ILogger<IncidentService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all incidents from the database.
    /// Ensures database is created before querying.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of all incidents.</returns>
    public async Task<IEnumerable<Incident>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving all incidents from database");
            
            // Create DbContext using factory (thread-safe, properly disposed)
            using var context = _contextFactory.CreateDbContext();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            // Return all incidents ordered by creation date (newest first)
            // SQLite doesn't support DateTimeOffset in ORDER BY, so we fetch then sort client-side
            var incidents = await context.Incidents
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            
            // Order on client side after data retrieval (SQLite limitation with DateTimeOffset)
            var orderedIncidents = incidents
                .OrderByDescending(i => i.CreatedDate)
                .ToList();
                
            _logger.LogInformation("Retrieved {IncidentCount} incidents from database", orderedIncidents.Count);
            return orderedIncidents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve incidents from database");
            throw;
        }
    }

    /// <summary>
    /// Creates a new incident in the database.
    /// </summary>
    /// <param name="incident">The incident to create.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created incident with assigned ID.</returns>
    public async Task<Incident> CreateAsync(Incident incident, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating new incident: {IncidentTitle}", incident.Title);
            
            // Create DbContext using factory (thread-safe, properly disposed)
            using var context = _contextFactory.CreateDbContext();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

            // Add the incident to the context
            context.Incidents.Add(incident);
            
            // Save changes to get the assigned ID
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            _logger.LogInformation("Successfully created incident with ID {IncidentId}: {IncidentTitle}", 
                incident.Id, incident.Title);
            
            return incident;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create incident: {IncidentTitle}", incident.Title);
            throw;
        }
    }
}
