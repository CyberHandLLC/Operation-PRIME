using OperationPrime.Domain.Enums;

namespace OperationPrime.Domain.Entities;

/// <summary>
/// MajorIncident entity for significant incidents with major impact (>100 users or critical systems).
/// Comprehensive incident type with full tracking, NOI generation, and bridge call management.
/// </summary>
public class MajorIncident : Incident
{
    /// <summary>
    /// Detailed comprehensive description of the issue (beyond the auto-generated template)
    /// </summary>
    public string DetailedDescription { get; private set; } = string.Empty;

    /// <summary>
    /// Whether this incident has been formally declared as a Major incident
    /// </summary>
    public bool IsDeclaredMajor { get; private set; }

    /// <summary>
    /// Date and time when the incident was declared as Major
    /// </summary>
    public DateTime? MajorDeclarationDate { get; private set; }

    /// <summary>
    /// Whether a bridge call has been established for this incident
    /// </summary>
    public bool HasBridgeCall { get; private set; }

    /// <summary>
    /// Bridge call URL if a bridge call is established
    /// Conditional field - only stored when HasBridgeCall is true
    /// </summary>
    public string? BridgeCallUrl { get; private set; }

    /// <summary>
    /// Whether an SME (Subject Matter Expert) has been contacted
    /// </summary>
    public bool SmeContacted { get; private set; }

    /// <summary>
    /// Name of the SME contacted (conditional on SmeContacted being true)
    /// Uses TextBox with autocomplete from team member list
    /// </summary>
    public string? SmeContactedName { get; private set; }

    /// <summary>
    /// Time when SME was contacted (auto-populated when SmeContacted is set to true)
    /// </summary>
    public DateTime? SmeContactedTime { get; private set; }

    /// <summary>
    /// Support team assigned to this incident
    /// Uses TextBox with autocomplete, can suggest team based on SME selection
    /// Conditional on SmeContacted being true
    /// </summary>
    public string? SupportTeam { get; private set; }

    /// <summary>
    /// Email distribution list for NOI and updates
    /// Team-based email suggestions with manual add/remove capability
    /// </summary>
    public string EmailDistributionList { get; private set; } = string.Empty;

    /// <summary>
    /// NOI (Notice of Incident) template used for this incident
    /// References .docx template file from templates folder
    /// </summary>
    public string? NoiTemplateName { get; private set; }

    /// <summary>
    /// Generated NOI content (populated from template with incident data)
    /// </summary>
    public string? NoiContent { get; private set; }

    /// <summary>
    /// Protected constructor for Entity Framework
    /// </summary>
    protected MajorIncident() : base() { }

    /// <summary>
    /// Creates a new MajorIncident with required properties
    /// </summary>
    public MajorIncident(
        string incidentNumber,
        string title,
        string applicationsAffected,
        string businessImpact,
        int impactedUsers,
        string locationsAffected,
        ImpactLevel impact,
        UrgencyLevel urgency,
        string createdBy,
        string? detailedDescription = null)
        : base(incidentNumber, IncidentType.MajorIncident, title, applicationsAffected, 
               businessImpact, impactedUsers, impact, urgency, createdBy)
    {
        DetailedDescription = detailedDescription ?? Description;
        
        // Set locations affected using reflection to access protected property
        var locationsField = typeof(Incident).GetField("<LocationsAffected>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        locationsField?.SetValue(this, locationsAffected ?? string.Empty);

        ValidateMajorIncidentRules();
    }

    /// <summary>
    /// Declares this incident as a Major incident
    /// </summary>
    public void DeclareAsMajor(string declaredBy)
    {
        IsDeclaredMajor = true;
        MajorDeclarationDate = DateTime.UtcNow;
        UpdateAuditTrail(declaredBy);
    }

    /// <summary>
    /// Establishes a bridge call for this incident
    /// </summary>
    public void EstablishBridgeCall(string bridgeUrl, string updatedBy)
    {
        HasBridgeCall = true;
        BridgeCallUrl = bridgeUrl ?? throw new ArgumentNullException(nameof(bridgeUrl));
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Records SME contact information
    /// </summary>
    public void ContactSme(string smeName, string? supportTeam, string updatedBy)
    {
        SmeContacted = true;
        SmeContactedName = smeName ?? throw new ArgumentNullException(nameof(smeName));
        SmeContactedTime = DateTime.UtcNow;
        SupportTeam = supportTeam;
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Updates the email distribution list
    /// </summary>
    public void UpdateEmailDistribution(string emailList, string updatedBy)
    {
        EmailDistributionList = emailList ?? throw new ArgumentNullException(nameof(emailList));
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Generates NOI using specified template
    /// </summary>
    public void GenerateNoi(string templateName, string noiContent, string updatedBy)
    {
        NoiTemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
        NoiContent = noiContent ?? throw new ArgumentNullException(nameof(noiContent));
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Updates the detailed description
    /// </summary>
    public void UpdateDetailedDescription(string newDetailedDescription, string updatedBy)
    {
        DetailedDescription = newDetailedDescription ?? throw new ArgumentNullException(nameof(newDetailedDescription));
        UpdateAuditTrail(updatedBy);
    }

    /// <summary>
    /// Validates business rules specific to MajorIncident
    /// </summary>
    private void ValidateMajorIncidentRules()
    {
        if (Type != IncidentType.MajorIncident)
        {
            throw new InvalidOperationException("MajorIncident must have IncidentType.MajorIncident");
        }

        // Major incidents typically have higher impact, but we don't enforce this strictly
        // to allow for edge cases and manual escalation
    }

    /// <summary>
    /// Override to ensure MajorIncident-specific validation
    /// </summary>
    protected override void ValidateBusinessRules()
    {
        base.ValidateBusinessRules();
        ValidateMajorIncidentRules();
    }
}