using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Unified incident entity supporting both pre-incidents and major incidents.
/// Follows the architectural decision for a single entity with type differentiation.
/// </summary>
public class Incident
{
    /// <summary>
    /// Unique identifier for the incident.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Brief title describing the incident.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Business impact description for Major Incidents.
    /// Describes what users cannot do due to the incident.
    /// Optional for Pre-Incidents, required for Major Incidents.
    /// </summary>
    public string? BusinessImpact { get; set; }

    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    public DateTime? TimeIssueStarted { get; set; }

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    public DateTime? TimeReported { get; set; }

    /// <summary>
    /// Description of users impacted by the incident.
    /// </summary>
    public string? ImpactedUsers { get; set; }

    /// <summary>
    /// Application(s) affected by the incident.
    /// </summary>
    public string? ApplicationAffected { get; set; }

    /// <summary>
    /// Location(s) affected by the incident.
    /// </summary>
    public string? LocationsAffected { get; set; }

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    public string? Workaround { get; set; }

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    public string? IncidentNumber { get; set; }

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    public int Urgency { get; set; } = 3;

    /// <summary>
    /// Type of incident (Pre-Incident or Major Incident).
    /// </summary>
    public IncidentType IncidentType { get; set; }

    /// <summary>
    /// Priority level of the incident (P1-P4).
    /// </summary>
    public Priority Priority { get; set; }

    /// <summary>
    /// Current status of the incident.
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// Date and time when the incident was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
