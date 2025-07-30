using OperationPrime.Domain.Enums;

namespace OperationPrime.Application.DTOs;

/// <summary>
/// Data Transfer Object for incident form data.
/// Follows Microsoft's 2024 MVVM Community Toolkit DTO patterns for clean data separation.
/// </summary>
public class IncidentFormData
{
    public string IncidentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentType IncidentType { get; set; } = IncidentType.PreIncident;
    public Priority Priority { get; set; } = Priority.P3;
    public Status Status { get; set; } = Status.New;
    public string BusinessImpact { get; set; } = string.Empty;
    public DateTimeOffset? TimeIssueStarted { get; set; }
    public DateTimeOffset? TimeReported { get; set; }
    public int? ImpactedUsers { get; set; }
    public ImpactedUsersCount? SelectedImpactedUsersCount { get; set; }
    public string Workaround { get; set; } = string.Empty;
    public string ApplicationAffected { get; set; } = string.Empty;
    public string LocationsAffected { get; set; } = string.Empty;
    public int Urgency { get; set; } = 3;
    
    // UI State properties (needed by IncidentDataMappingService)
    public int CurrentStep { get; set; } = 1;
    public bool IsSubmitting { get; set; } = false;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Computed property to determine if this is a major incident.
    /// </summary>
    public bool IsMajorIncident => IncidentType == IncidentType.MajorIncident;

    /// <summary>
    /// Validates if all required fields for the current incident type are filled.
    /// </summary>
    public bool IsValid()
    {
        // Basic required fields
        if (string.IsNullOrWhiteSpace(IncidentNumber) ||
            string.IsNullOrWhiteSpace(Title) ||
            string.IsNullOrWhiteSpace(Description) ||
            !TimeIssueStarted.HasValue ||
            !TimeReported.HasValue ||
            !ImpactedUsers.HasValue ||
            string.IsNullOrWhiteSpace(ApplicationAffected) ||
            string.IsNullOrWhiteSpace(LocationsAffected))
        {
            return false;
        }

        // Major incident specific validation
        if (IsMajorIncident && string.IsNullOrWhiteSpace(BusinessImpact))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Resets all form data to default values.
    /// </summary>
    public void Reset()
    {
        IncidentNumber = Title = Description = BusinessImpact = 
        ApplicationAffected = LocationsAffected = Workaround = string.Empty;
        
        IncidentType = IncidentType.PreIncident;
        Priority = Priority.P3;
        Status = Status.New;
        Urgency = 3;
        
        TimeIssueStarted = TimeReported = DateTimeOffset.UtcNow;
        ImpactedUsers = null;
        SelectedImpactedUsersCount = null;
        
        // Reset UI state
        CurrentStep = 1;
        IsSubmitting = false;
        ErrorMessage = null;
        SuccessMessage = null;
    }
}