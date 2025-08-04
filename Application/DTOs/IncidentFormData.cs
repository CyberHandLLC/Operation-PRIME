using OperationPrime.Domain.Enums;
using OperationPrime.Domain.Constants;

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
    public int Urgency { get; set; } = UrgencyLevels.Default;

    /// <summary>
    /// Computed property to determine if this is a major incident.
    /// </summary>
    public bool IsMajorIncident => IncidentType == IncidentType.MajorIncident;
}