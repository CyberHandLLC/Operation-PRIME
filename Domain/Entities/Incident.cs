using System.ComponentModel.DataAnnotations;
using OperationPrime.Domain.Enums;
using OperationPrime.Domain.ValueObjects;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Base class for all incident types containing common properties.
/// </summary>
public abstract class Incident : BaseEntity
{
    /// <summary>
    /// Gets or sets the incident number (unique identifier for display).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string IncidentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title or brief description of the incident.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the incident.
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of the incident.
    /// </summary>
    [Required]
    public IncidentStatus Status { get; set; } = IncidentStatus.New;

    /// <summary>
    /// Gets or sets the priority level of the incident.
    /// </summary>
    [Required]
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Gets or sets the urgency level of the incident.
    /// </summary>
    [Required]
    public UrgencyLevel Urgency { get; set; } = UrgencyLevel.Medium;

    /// <summary>
    /// Gets or sets the impact level of the incident.
    /// </summary>
    [Required]
    public ImpactLevel Impact { get; set; } = ImpactLevel.Medium;

    /// <summary>
    /// Gets or sets the type of incident.
    /// </summary>
    [Required]
    public abstract IncidentType Type { get; }

    /// <summary>
    /// Gets or sets the date and time when the incident occurred.
    /// </summary>
    [Required]
    public DateTime IncidentDateTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the affected application or system.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AffectedApplication { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the team responsible for support.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SupportTeam { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email addresses for notifications (semicolon-separated).
    /// </summary>
    [MaxLength(500)]
    public string? NotificationEmails { get; set; }

    /// <summary>
    /// Gets or sets whether a Notice of Incident (NOI) has been sent.
    /// </summary>
    public bool IsNoiSent { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the NOI was sent.
    /// </summary>
    public DateTime? NoiSentDateTime { get; set; }

    /// <summary>
    /// Gets or sets the resolution details.
    /// </summary>
    [MaxLength(2000)]
    public string? Resolution { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the incident was resolved.
    /// </summary>
    public DateTime? ResolvedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the incident was closed.
    /// </summary>
    public DateTime? ClosedDateTime { get; set; }

    /// <summary>
    /// Validates the incident data.
    /// </summary>
    /// <returns>True if valid, false otherwise.</returns>
    public virtual bool Validate()
    {
        // Basic validation - derived classes can extend
        if (string.IsNullOrWhiteSpace(IncidentNumber))
            return false;
        
        if (string.IsNullOrWhiteSpace(Title))
            return false;
        
        if (string.IsNullOrWhiteSpace(AffectedApplication))
            return false;
        
        if (string.IsNullOrWhiteSpace(SupportTeam))
            return false;

        // Status transitions validation
        if (Status == IncidentStatus.Resolved && !ResolvedDateTime.HasValue)
            return false;
        
        if (Status == IncidentStatus.Closed && !ClosedDateTime.HasValue)
            return false;

        return true;
    }

    /// <summary>
    /// Calculates the overall priority based on urgency and impact matrix.
    /// </summary>
    public void CalculatePriority()
    {
        var priorityMatrix = PriorityMatrix.Create(Urgency, Impact);
        Priority = priorityMatrix.CalculatedPriority;
    }

    /// <summary>
    /// Gets the priority calculation rationale for this incident.
    /// </summary>
    /// <returns>A string explaining the priority calculation.</returns>
    public string GetPriorityRationale()
    {
        var priorityMatrix = PriorityMatrix.Create(Urgency, Impact);
        return priorityMatrix.GetCalculationRationale();
    }
}
