using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for managing applications used in incident reporting.
/// Supports auto-suggestion functionality for Applications Affected field.
/// </summary>
public interface IApplicationService
{
    /// <summary>
    /// Retrieves all active applications for auto-suggestion.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of active applications.</returns>
    Task<IEnumerable<ApplicationInfo>> GetActiveApplicationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches applications by name for auto-suggestion.
    /// </summary>
    /// <param name="searchTerm">The search term to filter applications.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of matching applications.</returns>
    Task<IEnumerable<ApplicationInfo>> SearchApplicationsAsync(string searchTerm, int maxResults = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new application.
    /// </summary>
    /// <param name="application">The application to create.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created application with assigned ID.</returns>
    Task<ApplicationInfo> CreateAsync(ApplicationInfo application, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with common applications if empty.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Task representing the async operation.</returns>
    Task SeedApplicationsAsync(CancellationToken cancellationToken = default);
}
