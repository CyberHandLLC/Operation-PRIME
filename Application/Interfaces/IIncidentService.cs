using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for incident data operations.
/// Provides async methods for incident management following clean architecture principles.
/// </summary>
public interface IIncidentService
{
    /// <summary>
    /// Retrieves all incidents from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A collection of all incidents.</returns>
    Task<IEnumerable<Incident>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new incident in the database.
    /// </summary>
    /// <param name="incident">The incident to create.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created incident with assigned ID.</returns>
    Task<Incident> CreateAsync(Incident incident, CancellationToken cancellationToken = default);
}
