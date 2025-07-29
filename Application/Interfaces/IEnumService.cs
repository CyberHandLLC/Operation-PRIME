using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for providing enum collections.
/// Follows Clean Architecture principles by centralizing enum access in the domain layer.
/// Enables future enhancements like database-driven enum values.
/// </summary>
public interface IEnumService
{
    /// <summary>
    /// Gets all available incident types.
    /// </summary>
    /// <returns>Collection of incident types.</returns>
    IEnumerable<IncidentType> GetIncidentTypes();

    /// <summary>
    /// Gets all available priority levels.
    /// </summary>
    /// <returns>Collection of priority levels.</returns>
    IEnumerable<Priority> GetPriorities();

    /// <summary>
    /// Gets all available status values.
    /// </summary>
    /// <returns>Collection of status values.</returns>
    IEnumerable<Status> GetStatuses();
    
    /// <summary>
    /// Gets all available impacted users count values.
    /// </summary>
    /// <returns>Collection of impacted users count values.</returns>
    IEnumerable<ImpactedUsersCount> GetImpactedUsersCounts();
}
