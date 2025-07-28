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
    /// <returns>A collection of all incidents.</returns>
    Task<IEnumerable<Incident>> GetAllAsync();

    /// <summary>
    /// Creates a new incident in the database.
    /// </summary>
    /// <param name="incident">The incident to create.</param>
    /// <returns>The created incident with assigned ID.</returns>
    Task<Incident> CreateAsync(Incident incident);
}
