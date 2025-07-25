using OperationPrime.Domain.Entities;

namespace OperationPrime.Domain.Interfaces;

/// <summary>
/// Repository abstraction for <see cref="Incident"/> entities.
/// </summary>
public interface IIncidentRepository : IRepository<Incident>
{
    // Additional incident-specific queries can be added here
    Task<IEnumerable<Incident>> GetByStatusAsync(Enums.IncidentStatus status);
}
