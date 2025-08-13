using OperationPrime.Application.DTOs;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;
using OperationPrime.Domain.Constants;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for incident data mapping operations.
/// Extracted from IncidentCreateViewModel following Microsoft's 2024 MVVM Community Toolkit patterns.
/// </summary>
public class IncidentDataMappingService : IIncidentDataMappingService
{
    private readonly IDateTimeService _dateTimeService;

    public IncidentDataMappingService(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }
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
            CreatedDate = _dateTimeService.GetCurrentUtcTime(),

            // Impact Assessment
            ImpactedUsers = formData.ImpactedUsers,
            Urgency = formData.Urgency,
            BusinessImpact = formData.BusinessImpact,
            ApplicationAffected = formData.ApplicationAffected,
            LocationsAffected = formData.LocationsAffected,
            Workaround = formData.Workaround,

            // New fields
            IncidentSource = formData.IncidentSource,
            GeneratingMultipleCalls = formData.GeneratingMultipleCalls,

            // Timing
            TimeIssueStarted = formData.TimeIssueStarted,
            TimeReported = formData.TimeReported
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
            TimeIssueStarted = incident.TimeIssueStarted,
            TimeReported = incident.TimeReported,

            // New fields
            IncidentSource = incident.IncidentSource,
            GeneratingMultipleCalls = incident.GeneratingMultipleCalls
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
            Urgency = UrgencyLevels.Default,
            TimeIssueStarted = _dateTimeService.GetCurrentUtcTime(),
            TimeReported = _dateTimeService.GetCurrentUtcTime(),
            IncidentSource = IncidentSource.ServiceDesk,
            GeneratingMultipleCalls = GeneratingMultipleCalls.No
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
        formData.Urgency = UrgencyLevels.Default;

        // Reset date/time fields to current time
        formData.TimeIssueStarted = _dateTimeService.GetCurrentUtcTime();
        formData.TimeReported = _dateTimeService.GetCurrentUtcTime();

        // Reset nullable fields
        formData.ImpactedUsers = null;
        formData.SelectedImpactedUsersCount = null;

        // New fields
        formData.IncidentSource = IncidentSource.ServiceDesk;
        formData.GeneratingMultipleCalls = GeneratingMultipleCalls.No;


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
    public IncidentFormData MapFromViewModel(
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
        ImpactedUsersCount? selectedImpactedUsersCount,
        IncidentSource incidentSource,
        GeneratingMultipleCalls generatingMultipleCalls)
    {
        return new IncidentFormData
        {
            Title = title ?? string.Empty,
            Description = description ?? string.Empty,
            BusinessImpact = businessImpact ?? string.Empty,
            TimeIssueStarted = timeIssueStarted,
            TimeReported = timeReported,
            ImpactedUsers = impactedUsers,
            ApplicationAffected = applicationAffected ?? string.Empty,
            LocationsAffected = locationsAffected ?? string.Empty,
            Workaround = workaround ?? string.Empty,
            IncidentNumber = incidentNumber ?? string.Empty,
            Urgency = urgency,
            IncidentType = incidentType,
            Priority = priority,
            Status = status,
            SelectedImpactedUsersCount = selectedImpactedUsersCount,
            IncidentSource = incidentSource,
            GeneratingMultipleCalls = generatingMultipleCalls
        };
    }
}