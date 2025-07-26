using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Services;

/// <summary>
/// Basic workflow orchestration for pre-incident handling.
/// </summary>
public class PreIncidentWorkflowService : IPreIncidentWorkflowService
{
    private readonly IIncidentService _incidentService;
    private readonly ILogger<PreIncidentWorkflowService> _logger;

    public PreIncidentWorkflowService(IIncidentService incidentService, ILogger<PreIncidentWorkflowService> logger)
    {
        _incidentService = incidentService;
        _logger = logger;
    }

    public async Task<PreIncident> StartAsync(PreIncident incident)
    {
        _logger.LogDebug("Starting pre-incident workflow");
        await _incidentService.CreateAsync(incident);
        return incident;
    }

    public async Task<PreIncident> CompleteStepAsync(PreIncident incident, int stepIndex)
    {
        _logger.LogDebug("Completing step {Step} for pre-incident {Id}", stepIndex, incident.Id);
        await _incidentService.UpdateAsync(incident);
        return incident;
    }

    public async Task<MajorIncident> EscalateAsync(PreIncident incident, MajorIncident majorIncident)
    {
        _logger.LogInformation("Escalating pre-incident {Id} to major incident", incident.Id);
        incident.EscalateToMajorIncident(majorIncident.Id, majorIncident.CreatedBy);
        await _incidentService.UpdateAsync(incident);
        await _incidentService.CreateAsync(majorIncident);
        return majorIncident;
    }
}
