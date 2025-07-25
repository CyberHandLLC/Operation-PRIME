using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// PreIncident entity for minor incidents with limited impact (≤100 users).
/// Lightweight incident type with simplified workflow and fewer required fields.
/// Can be converted to MajorIncident if impact grows beyond 100 users.
/// </summary>
public class PreIncident : Incident
{
    /// <summary>
    /// Maximum allowed impacted users for PreIncident (per business rules)
    /// </summary>
    public const int MaxImpactedUsers = 100;

    /// <summary>
    /// Indicates if this PreIncident should be converted to MajorIncident
    /// Automatically set to true if impacted users > 100
    /// </summary>
    public bool ShouldConvertToMajor { get; private set; }

    /// <summary>
    /// Protected constructor for Entity Framework
    /// </summary>
    protected PreIncident() : base() { }

    /// <summary>
    /// Creates a new PreIncident with required properties
    /// </summary>
    public PreIncident(
        string incidentNumber,
        string title,
        string applicationsAffected,
        string businessImpact,
        int impactedUsers,
        ImpactLevel impact,
        UrgencyLevel urgency,
        string createdBy)
        : base(incidentNumber, IncidentType.PreIncident, title, applicationsAffected, 
               businessImpact, impactedUsers, impact, urgency, createdBy)
    {
        ValidatePreIncidentRules();
        CheckConversionThreshold();
    }

    /// <summary>
    /// Updates the number of impacted users and checks conversion threshold
    /// </summary>
    public void UpdateImpactedUsers(int newImpactedUsers, string updatedBy)
    {
        if (newImpactedUsers < 0)
        {
            throw new ArgumentException("Impacted users cannot be negative.", nameof(newImpactedUsers));
        }

        if (newImpactedUsers > 5000)
        {
            throw new ArgumentException("Impacted users cannot exceed 5,000 per validation rules.", nameof(newImpactedUsers));
        }

        // Use reflection to update the protected field
        var field = typeof(Incident).GetField("<ImpactedUsers>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(this, newImpactedUsers);

        CheckConversionThreshold();
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Checks if this PreIncident should be converted to MajorIncident
    /// Sets ShouldConvertToMajor flag based on business rules
    /// </summary>
    private void CheckConversionThreshold()
    {
        ShouldConvertToMajor = ImpactedUsers > MaxImpactedUsers;
    }

    /// <summary>
    /// Validates business rules specific to PreIncident
    /// </summary>
    private void ValidatePreIncidentRules()
    {
        // Note: We allow creation even if > 100 users, but flag for conversion
        // This supports the workflow where users might initially underestimate impact
        
        if (Type != IncidentType.PreIncident)
        {
            throw new InvalidOperationException("PreIncident must have IncidentType.PreIncident");
        }
    }

    /// <summary>
    /// Creates a MajorIncident from this PreIncident (for conversion workflow)
    /// </summary>
    public MajorIncident ConvertToMajorIncident(string convertedBy)
    {
        return new MajorIncident(
            incidentNumber: IncidentNumber,
            title: Title,
            applicationsAffected: ApplicationsAffected,
            businessImpact: BusinessImpact,
            impactedUsers: ImpactedUsers,
            locationsAffected: LocationsAffected,
            impact: PriorityMatrix.Impact,
            urgency: PriorityMatrix.Urgency,
            createdBy: convertedBy,
            detailedDescription: Description
        );
    }

    /// <summary>
    /// Override to ensure PreIncident-specific validation
    /// </summary>
    protected override void ValidateBusinessRules()
    {
        base.ValidateBusinessRules();
        ValidatePreIncidentRules();
    }
}