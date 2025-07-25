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
    private readonly IAuditService _auditService;

    public IncidentService(IIncidentRepository repository, IAuditService auditService, ILogger<IncidentService> logger)
    {
        _repository = repository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task CreateAsync(Incident incident)
    {
        _logger.LogDebug("Creating incident {IncidentNumber}", incident.IncidentNumber);
        await _repository.AddAsync(incident);
        await _auditService.LogActionAsync(incident.Id, "Created", "Incident created", incident.CreatedBy);
        _logger.LogInformation("Incident {Id} created", incident.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogDebug("Deleting incident {Id}", id);
        await _repository.DeleteAsync(id);
        await _auditService.LogActionAsync(id, "Deleted", "Incident deleted", "system");
        _logger.LogInformation("Incident {Id} deleted", id);
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all incidents");
        var result = await _repository.GetAllAsync();
        _logger.LogDebug("Retrieved {Count} incidents", result?.Count() ?? 0);
        return result;
    }

    public async Task<Incident?> GetAsync(Guid id)
    {
        _logger.LogDebug("Retrieving incident {Id}", id);
        return await _repository.GetAsync(id);
    }

    public async Task UpdateAsync(Incident incident)
    {
        _logger.LogDebug("Updating incident {Id}", incident.Id);
        await _repository.UpdateAsync(incident);
        await _auditService.LogActionAsync(incident.Id, "Updated", "Incident updated", incident.UpdatedBy);
        _logger.LogInformation("Incident {Id} updated", incident.Id);
    }
}
