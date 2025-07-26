using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Simple service interface for incident CRUD operations.
/// </summary>
public interface IIncidentService
{
    /// <summary>
    /// Gets all incidents in the system.
    /// </summary>
    /// <returns>A list of all incidents.</returns>
    Task<List<Incident>> GetAllAsync();

    /// <summary>
    /// Gets an incident by its unique identifier.
    /// </summary>
    /// <param name="id">The incident ID.</param>
    /// <returns>The incident if found, null otherwise.</returns>
    Task<Incident?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new incident.
    /// </summary>
    /// <param name="incident">The incident to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(Incident incident);

    /// <summary>
    /// Updates an existing incident.
    /// </summary>
    /// <param name="incident">The incident to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Incident incident);

    /// <summary>
    /// Deletes an incident by its unique identifier.
    /// </summary>
    /// <param name="id">The incident ID to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
