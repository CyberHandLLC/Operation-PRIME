using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Interfaces;

namespace OperationPrime.Application.Services;

/// <summary>
/// Application service providing higher level incident operations.
/// </summary>
public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _repository;
    private readonly ILogger<IncidentService> _logger;

    public IncidentService(IIncidentRepository repository, ILogger<IncidentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task CreateAsync(Incident incident)
    {
        await _repository.AddAsync(incident);
        _logger.LogInformation("Incident {Id} created", incident.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Incident {Id} deleted", id);
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Incident?> GetAsync(Guid id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task UpdateAsync(Incident incident)
    {
        await _repository.UpdateAsync(incident);
        _logger.LogInformation("Incident {Id} updated", incident.Id);
    }
}
