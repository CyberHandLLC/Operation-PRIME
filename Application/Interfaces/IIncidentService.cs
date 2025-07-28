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
}
