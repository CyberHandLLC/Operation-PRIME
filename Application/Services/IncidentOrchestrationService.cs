using Microsoft.Extensions.Logging;
using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Models;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for orchestrating complete incident operations.
/// Coordinates multiple services to handle complex business workflows.
/// </summary>
public class IncidentOrchestrationService : IIncidentOrchestrationService
{
    private readonly IIncidentService _incidentService;
    private readonly IIncidentValidationService _validationService;
    private readonly IIncidentDataMappingService _dataMappingService;
    private readonly ILogger<IncidentOrchestrationService> _logger;

    public IncidentOrchestrationService(
        IIncidentService incidentService,
        IIncidentValidationService validationService,
        IIncidentDataMappingService dataMappingService,
        ILogger<IncidentOrchestrationService> logger)
    {
        _incidentService = incidentService;
        _validationService = validationService;
        _dataMappingService = dataMappingService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new incident with full business logic validation and processing.
    /// </summary>
    public async Task<OperationResult<Incident>> CreateIncidentAsync(IncidentFormData formData, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting incident creation process for: {Title}", formData.Title);

            // Comprehensive business validation using consolidated validation service
            var validationResult = _validationService.ValidateCompleteIncidentData(formData);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Incident validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                return OperationResult<Incident>.ValidationFailure(validationResult.Errors);
            }

            // Map to domain entity using dedicated service
            var incident = _dataMappingService.MapToIncident(formData);

            // Save to persistence layer
            var createdIncident = await _incidentService.CreateAsync(incident, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Successfully created incident {IncidentId}: {Title}", createdIncident.Id, createdIncident.Title);
            return OperationResult<Incident>.Success(createdIncident);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Incident creation was cancelled");
            return OperationResult<Incident>.Failure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create incident: {Title}", formData.Title);
            return OperationResult<Incident>.Failure($"Failed to create incident: {ex.Message}");
        }
    }



    /// <summary>
    /// Initializes default values for a new incident form.
    /// Delegates to the mapping service to ensure a single source of truth for defaults.
    /// </summary>
    public IncidentFormData CreateDefaultIncidentForm()
    {
        return _dataMappingService.CreateDefaultFormData();
    }
} 