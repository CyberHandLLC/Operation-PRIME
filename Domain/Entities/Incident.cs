using System.ComponentModel.DataAnnotations;
using OperationPrime.Domain.Enums;
using OperationPrime.Domain.Constants;

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
    [StringLength(ValidationLengths.TitleMaxLength, ErrorMessage = "Title cannot exceed {1} characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident.
    /// </summary>
    [Required(ErrorMessage = "Incident description is required.")]
    [StringLength(ValidationLengths.DescriptionMaxLength, ErrorMessage = "Description cannot exceed {1} characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Business impact description for Major Incidents.
    /// Describes what users cannot do due to the incident.
    /// Optional for Pre-Incidents, required for Major Incidents.
    /// </summary>
    [StringLength(ValidationLengths.BusinessImpactMaxLength, ErrorMessage = "Business impact cannot exceed {1} characters.")]
    public string? BusinessImpact { get; set; }

    /// <summary>
    /// Date and time when the issue started.
    /// </summary>
    public DateTimeOffset? TimeIssueStarted { get; set; } 

    /// <summary>
    /// Date and time when the incident was reported.
    /// </summary>
    public DateTimeOffset? TimeReported { get; set; }

    /// <summary>
    /// Number of users impacted by the incident.
    /// Selected from predefined values: 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 500, 600, 800, 1000, 2000, 5000.
    /// </summary>
    [Range(1, 5000, ErrorMessage = "Impacted users must be between 1 and 5000.")]
    public int? ImpactedUsers { get; set; }

    /// <summary>
    /// Application(s) affected by the incident.
    /// </summary>
    [StringLength(ValidationLengths.ApplicationAffectedMaxLength, ErrorMessage = "Application affected cannot exceed {1} characters.")]
    public string? ApplicationAffected { get; set; }

    /// <summary>
    /// Location(s) affected by the incident.
    /// </summary>
    [StringLength(ValidationLengths.LocationsAffectedMaxLength, ErrorMessage = "Locations affected cannot exceed {1} characters.")]
    public string? LocationsAffected { get; set; }

    /// <summary>
    /// Available workaround for the incident (optional).
    /// </summary>
    [StringLength(ValidationLengths.WorkaroundMaxLength, ErrorMessage = "Workaround cannot exceed {1} characters.")]
    public string? Workaround { get; set; }

    /// <summary>
    /// Unique incident number for tracking.
    /// </summary>
    [Required(ErrorMessage = "Incident number is required.")]
    [StringLength(ValidationLengths.IncidentNumberMaxLength, ErrorMessage = "Incident number cannot exceed {1} characters.")]
    public string IncidentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Urgency level of the incident (1=High, 2=Medium, 3=Low).
    /// </summary>
    [Range(UrgencyLevels.Minimum, UrgencyLevels.Maximum, ErrorMessage = "Urgency must be between {1} (High) and {2} (Low).")]
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
