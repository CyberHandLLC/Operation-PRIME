using System.ComponentModel.DataAnnotations;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Represents a major incident - a significant disruption requiring immediate response.
/// </summary>
public class MajorIncident : Incident
{
    /// <summary>
    /// Gets the type of incident (always MajorIncident for this class).
    /// </summary>
    public override IncidentType Type => IncidentType.MajorIncident;

    /// <summary>
    /// Gets or sets the incident commander/manager name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string IncidentCommander { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the business impact description.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string BusinessImpact { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of affected users.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int AffectedUsersCount { get; set; }

    /// <summary>
    /// Gets or sets whether this is a customer-facing issue.
    /// </summary>
    public bool IsCustomerFacing { get; set; }

    /// <summary>
    /// Gets or sets the bridge/conference details for incident calls.
    /// </summary>
    [MaxLength(500)]
    public string? BridgeDetails { get; set; }

    /// <summary>
    /// Gets or sets the current status update for stakeholders.
    /// </summary>
    [MaxLength(1000)]
    public string? CurrentStatusUpdate { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last status update.
    /// </summary>
    public DateTime? LastStatusUpdateDateTime { get; set; }

    /// <summary>
    /// Gets or sets the root cause analysis.
    /// </summary>
    [MaxLength(2000)]
    public string? RootCause { get; set; }

    /// <summary>
    /// Gets or sets the lessons learned from this incident.
    /// </summary>
    [MaxLength(2000)]
    public string? LessonsLearned { get; set; }

    /// <summary>
    /// Gets or sets whether a post-incident review (PIR) is required.
    /// </summary>
    public bool IsPirRequired { get; set; }

    /// <summary>
    /// Gets or sets the date when PIR is scheduled.
    /// </summary>
    public DateTime? PirScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets whether the PIR has been completed.
    /// </summary>
    public bool IsPirCompleted { get; set; }

    /// <summary>
    /// Gets or sets the ID of the pre-incident if this was escalated.
    /// </summary>
    public Guid? EscalatedFromPreIncidentId { get; set; }

    /// <summary>
    /// Gets or sets the estimated time to resolution.
    /// </summary>
    public DateTime? EstimatedResolutionTime { get; set; }

    /// <summary>
    /// Gets or sets the actual downtime in minutes.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? DowntimeMinutes { get; set; }

    /// <summary>
    /// Validates the major incident data.
    /// </summary>
    /// <returns>True if valid, false otherwise.</returns>
    public override bool Validate()
    {
        if (!base.Validate())
            return false;

        if (string.IsNullOrWhiteSpace(IncidentCommander))
            return false;

        if (string.IsNullOrWhiteSpace(BusinessImpact))
            return false;

        // PIR validation
        if (IsPirRequired && IsPirCompleted && !PirScheduledDate.HasValue)
            return false;

        // Customer-facing incidents should have higher priority
        if (IsCustomerFacing && Priority == Priority.Low)
            return false;

        return true;
    }

    /// <summary>
    /// Updates the current status for stakeholder communication.
    /// </summary>
    /// <param name="statusUpdate">The new status update message.</param>
    /// <param name="updatedBy">The user updating the status.</param>
    public void UpdateStatus(string statusUpdate, string updatedBy)
    {
        CurrentStatusUpdate = statusUpdate;
        LastStatusUpdateDateTime = DateTime.UtcNow;
        UpdateAuditFields(updatedBy);
    }

    /// <summary>
    /// Marks the incident as resolved with resolution details.
    /// </summary>
    /// <param name="resolution">The resolution description.</param>
    /// <param name="rootCause">The root cause analysis.</param>
    /// <param name="resolvedBy">The user who resolved the incident.</param>
    public void ResolveIncident(string resolution, string? rootCause, string resolvedBy)
    {
        Status = IncidentStatus.Resolved;
        Resolution = resolution;
        RootCause = rootCause;
        ResolvedDateTime = DateTime.UtcNow;
        
        // Calculate downtime if possible
        if (IncidentDateTime != default)
        {
            var downtime = DateTime.UtcNow - IncidentDateTime;
            DowntimeMinutes = (int)downtime.TotalMinutes;
        }
        
        UpdateAuditFields(resolvedBy);
    }
}
