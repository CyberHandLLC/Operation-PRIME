using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Services;

/// <summary>
/// Basic workflow orchestration for major incident management.
/// </summary>
public class MajorIncidentWorkflowService : IMajorIncidentWorkflowService
{
    private readonly IIncidentService _incidentService;
    private readonly INOIService _noiService;
    private readonly ILogger<MajorIncidentWorkflowService> _logger;

    public MajorIncidentWorkflowService(IIncidentService incidentService, INOIService noiService, ILogger<MajorIncidentWorkflowService> logger)
    {
        _incidentService = incidentService;
        _noiService = noiService;
        _logger = logger;
    }

    public async Task<MajorIncident> StartAsync(MajorIncident incident)
    {
        _logger.LogDebug("Starting major incident workflow");
        await _incidentService.CreateAsync(incident);
        return incident;
    }

    public async Task<MajorIncident> UpdateStatusAsync(MajorIncident incident, string statusUpdate, string updatedBy)
    {
        incident.UpdateStatus(statusUpdate, updatedBy);
        await _incidentService.UpdateAsync(incident);
        return incident;
    }

    // Future asynchronous version placeholder. Once template rendering or
    // email sending becomes asynchronous, uncomment and implement accordingly.
    // public async Task<string> GenerateNoiAsync(MajorIncident incident, string templateType)
    // {
    //     _logger.LogInformation("Generating NOI for incident {Id}", incident.Id);
    //     var content = await SomeAsyncNoiGenerationMethod(incident, templateType);
    //     return content;
    // }
    /// <summary>
    /// Generates NOI content for a major incident.
    /// TODO: Make this method truly async when NOI generation requires I/O or external calls.
    /// </summary>
    public Task<string> GenerateNoiAsync(MajorIncident incident, string templateType)
    {
        _logger.LogInformation("Generating NOI for incident {Id}", incident.Id);
        var content = _noiService.GenerateNOI(incident, templateType);
        return Task.FromResult(content);
    }
    // The following async version is commented out as a placeholder for future async implementation.
    // It is currently redundant, but left here for AI agents or developers to reference or correct later.
    /*
    public async Task<string> GenerateNoiAsync(MajorIncident incident, string templateType)
    {
        _logger.LogInformation("Generating NOI for incident {Id}", incident.Id);
        var content = _noiService.GenerateNOI(incident, templateType);
        return content;
    }
    */
}
