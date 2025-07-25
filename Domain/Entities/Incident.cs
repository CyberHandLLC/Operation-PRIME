using OperationPrime.Domain.Enums;
using OperationPrime.Domain.ValueObjects;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// Base incident entity containing common properties for all incident types.
/// Serves as the foundation for PreIncident and MajorIncident entities.
/// </summary>
public class Incident : BaseEntity
{
    /// <summary>
    /// Human-readable incident number (e.g., INC-2024-001)
    /// </summary>
    public string IncidentNumber { get; protected set; } = string.Empty;

    /// <summary>
    /// Type of incident (PreIncident or MajorIncident)
    /// </summary>
    public IncidentType Type { get; protected set; }

    /// <summary>
    /// Current status of the incident
    /// </summary>
    public IncidentStatus Status { get; protected set; } = IncidentStatus.New;

    /// <summary>
    /// Brief title/summary of the incident
    /// </summary>
    public string Title { get; protected set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident
    /// Auto-generated template: "{ApplicationsAffected} users are experiencing issues. The business impact is that {BusinessImpact}."
    /// Can be overridden manually.
    /// </summary>
    public string Description { get; protected set; } = string.Empty;

    /// <summary>
    /// Priority matrix calculation based on impact and urgency
    /// </summary>
    public PriorityMatrix PriorityMatrix { get; protected set; }

    /// <summary>
    /// Applications affected by this incident (comma-separated or structured)
    /// Uses TextBox with autocomplete from application list
    /// </summary>
    public string ApplicationsAffected { get; protected set; } = string.Empty;

    /// <summary>
    /// Business impact description
    /// </summary>
    public string BusinessImpact { get; protected set; } = string.Empty;

    /// <summary>
    /// Number of users impacted (max 5,000 per validation rules)
    /// </summary>
    public int ImpactedUsers { get; protected set; }

    /// <summary>
    /// Geographic or network locations affected
    /// </summary>
    public string LocationsAffected { get; protected set; } = string.Empty;

    /// <summary>
    /// Protected constructor for entity framework and derived classes
    /// </summary>
    protected Incident() { }

    /// <summary>
    /// Creates a new incident with required properties
    /// </summary>
    public Incident(
        string incidentNumber,
        IncidentType type,
        string title,
        string applicationsAffected,
        string businessImpact,
        int impactedUsers,
        ImpactLevel impact,
        UrgencyLevel urgency,
        string createdBy)
    {
        IncidentNumber = incidentNumber ?? throw new ArgumentNullException(nameof(incidentNumber));
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ApplicationsAffected = applicationsAffected ?? throw new ArgumentNullException(nameof(applicationsAffected));
        BusinessImpact = businessImpact ?? throw new ArgumentNullException(nameof(businessImpact));
        ImpactedUsers = impactedUsers;
        PriorityMatrix = new PriorityMatrix(impact, urgency);
        
        // Auto-generate description using template
        Description = GenerateDescription(applicationsAffected, businessImpact);
        
        SetCreator(createdBy);
        ValidateBusinessRules();
    }

    /// <summary>
    /// Updates incident status with audit trail
    /// </summary>
    public virtual void UpdateStatus(IncidentStatus newStatus, string updatedBy)
    {
        Status = newStatus;
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Updates the description (allows manual override of auto-generated template)
    /// </summary>
    public virtual void UpdateDescription(string newDescription, string updatedBy)
    {
        Description = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Updates priority matrix and recalculates priority
    /// </summary>
    public virtual void UpdatePriority(ImpactLevel impact, UrgencyLevel urgency, string updatedBy)
    {
        PriorityMatrix = new PriorityMatrix(impact, urgency);
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Generates auto-description using template pattern
    /// </summary>
    private static string GenerateDescription(string applicationsAffected, string businessImpact)
    {
        return $"{applicationsAffected} users are experiencing issues. The business impact is that {businessImpact}.";
    }

    /// <summary>
    /// Validates business rules for the incident
    /// </summary>
    protected virtual void ValidateBusinessRules()
    {
        if (ImpactedUsers > 5000)
        {
            throw new ArgumentException("Impacted users cannot exceed 5,000 per validation rules.", nameof(ImpactedUsers));
        }

        if (ImpactedUsers < 0)
        {
            throw new ArgumentException("Impacted users cannot be negative.", nameof(ImpactedUsers));
        }

        if (string.IsNullOrWhiteSpace(IncidentNumber))
        {
            throw new ArgumentException("Incident number is required.", nameof(IncidentNumber));
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new ArgumentException("Incident title is required.", nameof(Title));
        }
    }
}