using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Service implementation for providing enum collections.
/// Follows Clean Architecture principles and Microsoft best practices.
/// Can be extended in the future to load enum values from database or external sources.
/// </summary>
public class EnumService : IEnumService
{
    /// <summary>
    /// Gets all available incident types.
    /// </summary>
    /// <returns>Collection of incident types.</returns>
    public IEnumerable<IncidentType> GetIncidentTypes()
    {
        return Enum.GetValues<IncidentType>();
    }

    /// <summary>
    /// Gets all available priority levels.
    /// </summary>
    /// <returns>Collection of priority levels.</returns>
    public IEnumerable<Priority> GetPriorities()
    {
        return Enum.GetValues<Priority>();
    }

    /// <summary>
    /// Gets all available status values.
    /// </summary>
    /// <returns>Collection of status values.</returns>
    public IEnumerable<Status> GetStatuses()
    {
        return Enum.GetValues<Status>();
    }
    
    /// <summary>
    /// Gets all available impacted users count values.
    /// </summary>
    /// <returns>Collection of impacted users count values.</returns>
    public IEnumerable<ImpactedUsersCount> GetImpactedUsersCounts()
    {
        return Enum.GetValues<ImpactedUsersCount>();
    }
}
