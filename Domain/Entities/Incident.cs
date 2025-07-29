using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "Incident title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [Required(ErrorMessage = "Incident description is required.")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Business impact description for Major Incidents.
    /// Describes what users cannot do due to the incident.
    /// Optional for Pre-Incidents, required for Major Incidents.
    /// </summary>
    [StringLength(500, ErrorMessage = "Business impact cannot exceed 500 characters.")]
    public string? BusinessImpact { get; set; }

    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    public DateTimeOffset? TimeIssueStarted { get; set; } 

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    public DateTimeOffset? TimeReported { get; set; } // Using a ? means that the field can be empty, why would this field be empty?

    /// <summary>
    /// Number of users impacted by the incident.
    /// Selected from predefined values: 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 500, 600, 800, 1000, 2000, 5000.
    /// </summary>
    [Range(1, 5000, ErrorMessage = "Impacted users must be between 1 and 5000.")]
    public int? ImpactedUsers { get; set; }

    /// <summary>
    /// Application(s) affected by the incident.
    /// </summary>
    [StringLength(200, ErrorMessage = "Application affected cannot exceed 200 characters.")]
    public string? ApplicationAffected { get; set; }

    /// <summary>
    /// Location(s) affected by the incident.
    /// </summary>
    [StringLength(300, ErrorMessage = "Locations affected cannot exceed 300 characters.")]
    public string? LocationsAffected { get; set; }

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [StringLength(500, ErrorMessage = "Workaround cannot exceed 500 characters.")]
    public string? Workaround { get; set; }

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [StringLength(50, ErrorMessage = "Incident number cannot exceed 50 characters.")]
    public string? IncidentNumber { get; set; }

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    [Range(1, 3, ErrorMessage = "Urgency must be between 1 (High) and 3 (Low).")]
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
    public DateTimeOffset CreatedDate { get; set; }
}
