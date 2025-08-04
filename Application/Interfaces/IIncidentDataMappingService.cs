using OperationPrime.Application.DTOs;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for incident data mapping operations.
/// Follows Microsoft's 2024 MVVM Community Toolkit service extraction patterns.
/// </summary>
public interface IIncidentDataMappingService
{
    /// <summary>
    /// Maps form data to an Incident entity for creation
    /// </summary>
    /// <param name="formData">The form data from the ViewModel</param>
    /// <returns>A new Incident entity ready for persistence</returns>
    Incident MapToIncident(IncidentFormData formData);

    /// <summary>
    /// Maps an existing Incident entity to form data for editing
    /// </summary>
    /// <param name="incident">The existing incident entity</param>
    /// <returns>Form data populated from the incident</returns>
    IncidentFormData MapFromIncident(Incident incident);

    /// <summary>
    /// Creates a new IncidentFormData with default values
    /// </summary>
    /// <returns>Form data with appropriate defaults</returns>
    IncidentFormData CreateDefaultFormData();

    /// <summary>
    /// Resets form data to default values
    /// </summary>
    /// <param name="formData">The form data to reset</param>
    void ResetFormData(IncidentFormData formData);

    /// <summary>
    /// Maps ViewModel data to IncidentFormData DTO.
    /// Handles the conversion from Presentation layer data to Application layer DTO.
    /// Follows Clean Architecture by keeping DTO creation in Application layer.
    /// </summary>
    /// <param name="title">Incident title</param>
    /// <param name="description">Incident description</param>
    /// <param name="businessImpact">Business impact description</param>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <param name="impactedUsers">Number of impacted users</param>
    /// <param name="applicationAffected">Affected application</param>
    /// <param name="locationsAffected">Affected locations</param>
    /// <param name="workaround">Available workaround</param>
    /// <param name="incidentNumber">Incident tracking number</param>
    /// <param name="urgency">Urgency level</param>
    /// <param name="incidentType">Type of incident</param>
    /// <param name="priority">Incident priority</param>
    /// <param name="status">Current status</param>
    /// <param name="selectedImpactedUsersCount">Selected users count enum</param>
    /// <returns>IncidentFormData with all properties mapped</returns>
    IncidentFormData MapFromViewModel(
        string? title,
        string? description, 
        string? businessImpact,
        DateTimeOffset? timeIssueStarted,
        DateTimeOffset? timeReported,
        int? impactedUsers,
        string? applicationAffected,
        string? locationsAffected,
        string? workaround,
        string? incidentNumber,
        int urgency,
        IncidentType incidentType,
        Priority priority,
        Status status,
        ImpactedUsersCount? selectedImpactedUsersCount);
}