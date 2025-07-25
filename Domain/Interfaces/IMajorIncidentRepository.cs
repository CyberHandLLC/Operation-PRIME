using OperationPrime.Domain.Entities;

namespace OperationPrime.Domain.Interfaces;

/// <summary>
/// Repository abstraction for <see cref="MajorIncident"/> entities.
/// </summary>
public interface IMajorIncidentRepository : IRepository<MajorIncident>
{
    // Additional major-incident specific queries can be added here
    Task<IEnumerable<MajorIncident>> GetOpenAsync();
}
