using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Application.Models;
using OperationPrime.Domain.Enums;
using OperationPrime.Domain.Constants;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for incident validation logic.
/// Extracted from IncidentCreateViewModel following Microsoft's 2024 MVVM Community Toolkit patterns.
/// Consolidated validation logic to eliminate duplication with IncidentOrchestrationService.
/// </summary>
public class IncidentValidationService : IIncidentValidationService
{
    private readonly IDateTimeService _dateTimeService;

    public IncidentValidationService(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }
    /// <summary>
    /// Validates Step 1: Incident Type Selection
    /// </summary>
    public bool ValidateStep1(IncidentType incidentType)
    {
        // Step 1 is always valid as IncidentType has a default value
        return true;
    }

    /// <summary>
    /// Validates Step 2: Impact Assessment
    /// </summary>
    public bool ValidateStep2(int? impactedUsers, int urgency, string? businessImpact, 
        string? applicationAffected, string? locationsAffected, string? workaround)
    {
        // Step 2 fields required for Pre-Incident workflow
        // Note: businessImpact and workaround are NOT required in Step 2
        // Workaround is optional (marked as "Workaround (Optional)" in UI)
        return impactedUsers.HasValue &&
               urgency >= UrgencyLevels.Minimum && urgency <= UrgencyLevels.Maximum &&
               !string.IsNullOrWhiteSpace(applicationAffected) &&
               !string.IsNullOrWhiteSpace(locationsAffected);
    }

    /// <summary>
    /// Validates Step 3: Incident Details
    /// </summary>
    public bool ValidateStep3(string? incidentNumber, string? title, string? description, 
        DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported)
    {
        // Core incident details are required
        return !string.IsNullOrWhiteSpace(incidentNumber) &&
               !string.IsNullOrWhiteSpace(title) &&
               !string.IsNullOrWhiteSpace(description) &&
               timeIssueStarted.HasValue &&
               timeReported.HasValue;
    }

    /// <summary>
    /// Validates Step 4: Master Checklist (Major Incidents only)
    /// </summary>
    public bool ValidateStep4(IncidentType incidentType, Priority priority, Status status)
    {
        // Step 4 is only required for Major Incidents
        if (incidentType == IncidentType.PreIncident)
        {
            return true; // Pre-incidents don't have Step 4
        }

        // Major incidents require valid priority and status
        return Enum.IsDefined(typeof(Priority), priority) &&
               Enum.IsDefined(typeof(Status), status);
    }



    /// <summary>
    /// Validates a specific step using form data.
    /// Follows 2024 best practices for simplified validation delegation.
    /// </summary>
    public bool ValidateCurrentStep(int currentStep, IncidentFormData formData)
    {
        return currentStep switch
        {
            1 => ValidateStep1(formData.IncidentType),
            2 => ValidateStep2(formData.ImpactedUsers, formData.Urgency, formData.BusinessImpact, 
                    formData.ApplicationAffected, formData.LocationsAffected, formData.Workaround),
            3 => ValidateStep3(formData.IncidentNumber, formData.Title, formData.Description, 
                    formData.TimeIssueStarted, formData.TimeReported),
            4 => ValidateStep4(formData.IncidentType, formData.Priority, formData.Status),
            _ => false
        };
    }

    /// <summary>
    /// Validates complete incident form data across all business rules.
    /// Consolidates validation logic that was previously duplicated in IncidentOrchestrationService.
    /// Provides detailed error messages for comprehensive validation feedback.
    /// </summary>
    /// <param name="formData">The incident form data to validate</param>
    /// <returns>Validation result with detailed error information</returns>
    public ValidationResult ValidateCompleteIncidentData(IncidentFormData formData)
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

        if (formData.Urgency < UrgencyLevels.Minimum || formData.Urgency > UrgencyLevels.Maximum)
            errors.Add($"Urgency must be between {UrgencyLevels.Minimum} and {UrgencyLevels.Maximum}");

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }
}