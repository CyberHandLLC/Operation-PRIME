using OperationPrime.Application.Models;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for incident validation logic.
/// Follows Microsoft's 2024 MVVM Community Toolkit service extraction patterns.
/// </summary>
public interface IIncidentValidationService
{
    /// <summary>
    /// Validates Step 1: Incident Type Selection
    /// </summary>
    /// <param name="incidentType">The selected incident type</param>
    /// <returns>True if step is valid</returns>
    bool ValidateStep1(IncidentType incidentType);

    /// <summary>
    /// Validates Step 2: Impact Assessment
    /// </summary>
    /// <param name="impactedUsers">Number of impacted users</param>
    /// <param name="urgency">Urgency level (1-5)</param>
    /// <param name="businessImpact">Business impact description</param>
    /// <param name="applicationAffected">Affected application</param>
    /// <param name="locationsAffected">Affected locations</param>
    /// <param name="workaround">Workaround description</param>
    /// <returns>True if step is valid</returns>
    bool ValidateStep2(int? impactedUsers, int urgency, string? businessImpact, 
        string? applicationAffected, string? locationsAffected, string? workaround);

    /// <summary>
    /// Validates Step 3: Incident Details
    /// </summary>
    /// <param name="incidentNumber">Incident number</param>
    /// <param name="title">Incident title</param>
    /// <param name="description">Incident description</param>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <returns>True if step is valid</returns>
    bool ValidateStep3(string? incidentNumber, string? title, string? description, 
        DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported);

    /// <summary>
    /// Validates Step 4: Master Checklist (Major Incidents only)
    /// </summary>
    /// <param name="incidentType">The incident type</param>
    /// <param name="priority">Incident priority</param>
    /// <param name="status">Incident status</param>
    /// <returns>True if step is valid</returns>
    bool ValidateStep4(IncidentType incidentType, Priority priority, Status status);



    /// <summary>
    /// Validates a specific step using form data.
    /// Follows 2024 best practices for simplified validation delegation.
    /// </summary>
    /// <param name="currentStep">The step to validate</param>
    /// <param name="formData">Complete form data</param>
    /// <returns>True if the current step is valid</returns>
    bool ValidateCurrentStep(int currentStep, Application.DTOs.IncidentFormData formData);

    /// <summary>
    /// Validates complete incident form data across all business rules.
    /// Consolidates validation logic to eliminate duplication with orchestration services.
    /// Provides detailed error messages for comprehensive validation feedback.
    /// </summary>
    /// <param name="formData">The incident form data to validate</param>
    /// <returns>Validation result with detailed error information</returns>
    ValidationResult ValidateCompleteIncidentData(Application.DTOs.IncidentFormData formData);
}