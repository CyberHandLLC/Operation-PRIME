using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for incident data mapping operations.
/// Extracted from IncidentCreateViewModel following Microsoft's 2024 MVVM Community Toolkit patterns.
/// </summary>
public class IncidentDataMappingService : IIncidentDataMappingService
{
    /// <summary>
    /// Maps form data to an Incident entity for creation
    /// </summary>
    public Incident MapToIncident(IncidentFormData formData)
    {
        return new Incident
        {
            // Core Properties
            IncidentNumber = formData.IncidentNumber,
            Title = formData.Title,
            Description = formData.Description,
            IncidentType = formData.IncidentType,
            Priority = formData.Priority,
            Status = formData.Status,
            CreatedDate = DateTime.UtcNow,

            // Impact Assessment
            ImpactedUsers = formData.ImpactedUsers,
            Urgency = formData.Urgency,
            BusinessImpact = formData.BusinessImpact,
            ApplicationAffected = formData.ApplicationAffected,
            LocationsAffected = formData.LocationsAffected,
            Workaround = formData.Workaround,

            // Timing
            TimeIssueStarted = formData.TimeIssueStarted?.DateTime,
            TimeReported = formData.TimeReported?.DateTime
        };
    }

    /// <summary>
    /// Maps an existing Incident entity to form data for editing
    /// </summary>
    public IncidentFormData MapFromIncident(Incident incident)
    {
        return new IncidentFormData
        {
            // Core Properties
            IncidentNumber = incident.IncidentNumber ?? string.Empty,
            Title = incident.Title ?? string.Empty,
            Description = incident.Description ?? string.Empty,
            IncidentType = incident.IncidentType,
            Priority = incident.Priority,
            Status = incident.Status,

            // Impact Assessment
            ImpactedUsers = incident.ImpactedUsers,
            SelectedImpactedUsersCount = ConvertToImpactedUsersCount(incident.ImpactedUsers),
            Urgency = incident.Urgency,
            BusinessImpact = incident.BusinessImpact ?? string.Empty,
            ApplicationAffected = incident.ApplicationAffected ?? string.Empty,
            LocationsAffected = incident.LocationsAffected ?? string.Empty,
            Workaround = incident.Workaround ?? string.Empty,

            // Timing
            TimeIssueStarted = incident.TimeIssueStarted?.ToUniversalTime(),
            TimeReported = incident.TimeReported?.ToUniversalTime(),

            // UI State
            CurrentStep = 1,
            IsSubmitting = false,
            ErrorMessage = null,
            SuccessMessage = null
        };
    }

    /// <summary>
    /// Creates a new IncidentFormData with default values
    /// </summary>
    public IncidentFormData CreateDefaultFormData()
    {
        return new IncidentFormData
        {
            IncidentType = IncidentType.PreIncident,
            Priority = Priority.P3,
            Status = Status.New,
            Urgency = 3,
            TimeIssueStarted = DateTimeOffset.UtcNow,
            TimeReported = DateTimeOffset.UtcNow,
            CurrentStep = 1,
            IsSubmitting = false
        };
    }

    /// <summary>
    /// Resets form data to default values
    /// </summary>
    public void ResetFormData(IncidentFormData formData)
    {
        // Reset text fields
        formData.Title = string.Empty;
        formData.Description = string.Empty;
        formData.BusinessImpact = string.Empty;
        formData.ApplicationAffected = string.Empty;
        formData.LocationsAffected = string.Empty;
        formData.Workaround = string.Empty;
        formData.IncidentNumber = string.Empty;

        // Reset enums to defaults
        formData.IncidentType = IncidentType.PreIncident;
        formData.Priority = Priority.P3;
        formData.Status = Status.New;
        formData.Urgency = 3;

        // Reset date/time fields to current time
        formData.TimeIssueStarted = DateTimeOffset.UtcNow;
        formData.TimeReported = DateTimeOffset.UtcNow;

        // Reset nullable fields
        formData.ImpactedUsers = null;
        formData.SelectedImpactedUsersCount = null;

        // Reset UI state
        formData.CurrentStep = 1;
        formData.IsSubmitting = false;
        formData.ErrorMessage = null;
        formData.SuccessMessage = null;
    }

    /// <summary>
    /// Converts integer impacted users to enum value
    /// </summary>
    private static ImpactedUsersCount? ConvertToImpactedUsersCount(int? impactedUsers)
    {
        return impactedUsers switch
        {
            <= 5 => ImpactedUsersCount.Five,
            <= 10 => ImpactedUsersCount.Ten,
            <= 20 => ImpactedUsersCount.Twenty,
            <= 30 => ImpactedUsersCount.Thirty,
            <= 40 => ImpactedUsersCount.Forty,
            <= 50 => ImpactedUsersCount.Fifty,
            <= 60 => ImpactedUsersCount.Sixty,
            <= 70 => ImpactedUsersCount.Seventy,
            <= 80 => ImpactedUsersCount.Eighty,
            <= 90 => ImpactedUsersCount.Ninety,
            <= 100 => ImpactedUsersCount.OneHundred,
            <= 200 => ImpactedUsersCount.TwoHundred,
            <= 300 => ImpactedUsersCount.ThreeHundred,
            <= 500 => ImpactedUsersCount.FiveHundred,
            <= 600 => ImpactedUsersCount.SixHundred,
            <= 800 => ImpactedUsersCount.EightHundred,
            <= 1000 => ImpactedUsersCount.OneThousand,
            <= 2000 => ImpactedUsersCount.TwoThousand,
            > 2000 => ImpactedUsersCount.FiveThousand,
            _ => null
        };
    }
}