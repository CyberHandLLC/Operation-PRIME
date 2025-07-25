using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Application service for managing incidents.
/// </summary>
public interface IIncidentService
{
    Task<Incident?> GetAsync(Guid id);
    Task<IEnumerable<Incident>> GetAllAsync();
    Task CreateAsync(Incident incident);
    Task UpdateAsync(Incident incident);
    Task DeleteAsync(Guid id);
}
