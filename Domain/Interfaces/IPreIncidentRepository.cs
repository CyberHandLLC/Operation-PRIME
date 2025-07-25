using OperationPrime.Domain.Entities;

namespace OperationPrime.Domain.Interfaces;

/// <summary>
/// Repository abstraction for <see cref="PreIncident"/> entities.
/// </summary>
public interface IPreIncidentRepository : IRepository<PreIncident>
{
    // Additional pre-incident specific queries can be added here
    Task<IEnumerable<PreIncident>> GetEscalatedAsync();
}
