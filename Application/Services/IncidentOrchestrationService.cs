using Microsoft.Extensions.Logging;
using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

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
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<IncidentOrchestrationService> _logger;

    public IncidentOrchestrationService(
        IIncidentService incidentService,
        IIncidentValidationService validationService,
        IIncidentDataMappingService dataMappingService,
        IDateTimeService dateTimeService,
        ILogger<IncidentOrchestrationService> logger)
    {
        _incidentService = incidentService;
        _validationService = validationService;
        _dataMappingService = dataMappingService;
        _dateTimeService = dateTimeService;
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

            // Comprehensive business validation
            var validationResult = ValidateIncidentData(formData);
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
    /// Validates complete incident form data across all business rules.
    /// </summary>
    public ValidationResult ValidateIncidentData(IncidentFormData formData)
    {
        var errors = new List<string>();

        // Basic required field validation
        if (string.IsNullOrWhiteSpace(formData.Title))
            errors.Add("Incident title is required");

        if (string.IsNullOrWhiteSpace(formData.Description))
            errors.Add("Incident description is required");
            
        if (string.IsNullOrWhiteSpace(formData.IncidentNumber))
            errors.Add("Incident number is required");

        if (string.IsNullOrWhiteSpace(formData.ApplicationAffected))
            errors.Add("Application affected is required");

        if (string.IsNullOrWhiteSpace(formData.LocationsAffected))
            errors.Add("Locations affected is required");

        // Business rule: Major incidents require business impact
        if (formData.IncidentType == IncidentType.MajorIncident && string.IsNullOrWhiteSpace(formData.BusinessImpact))
            errors.Add("Business Impact is required for Major Incidents");

        // Date/time validation using dedicated service
        if (!_dateTimeService.ValidateIssueStartTime(formData.TimeIssueStarted))
            errors.Add("Issue start time cannot be in the future");

        if (!_dateTimeService.ValidateReportedTime(formData.TimeIssueStarted, formData.TimeReported))
            errors.Add("Reported time must be after or equal to issue start time");

        // Numeric validation
        if (!formData.ImpactedUsers.HasValue || formData.ImpactedUsers.Value <= 0)
            errors.Add("Number of impacted users is required");

        if (formData.Urgency < 1 || formData.Urgency > 5)
            errors.Add("Urgency must be between 1 and 5");

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }

    /// <summary>
    /// Initializes default values for a new incident form.
    /// </summary>
    public IncidentFormData CreateDefaultIncidentForm()
    {
        var currentTime = _dateTimeService.GetCurrentUtcTime();
        
        return new IncidentFormData
        {
            IncidentType = IncidentType.PreIncident,
            Priority = Priority.P3,
            Status = Status.New,
            Urgency = 3,
            TimeIssueStarted = currentTime,
            TimeReported = currentTime,
            CurrentStep = 1,
            IsSubmitting = false
        };
    }
} 