namespace OperationPrime.Domain.Constants;

/// <summary>
/// Domain-level validation constants for incident fields.
/// Centralized to ensure consistency across all layers following Clean Architecture principles.
/// </summary>
public static class ValidationLengths
{
    /// <summary>
    /// Maximum length for incident title.
    /// </summary>
    public const int TitleMaxLength = 200;
    
    /// <summary>
    /// Maximum length for incident description.
    /// </summary>
    public const int DescriptionMaxLength = 2000;
    
    /// <summary>
    /// Maximum length for business impact description.
    /// </summary>
    public const int BusinessImpactMaxLength = 1000;
    
    /// <summary>
    /// Maximum length for application affected field.
    /// </summary>
    public const int ApplicationAffectedMaxLength = 200;
    
    /// <summary>
    /// Maximum length for locations affected field.
    /// </summary>
    public const int LocationsAffectedMaxLength = 300;
    
    /// <summary>
    /// Maximum length for workaround field.
    /// </summary>
    public const int WorkaroundMaxLength = 500;
    
    /// <summary>
    /// Maximum length for incident number.
    /// </summary>
    public const int IncidentNumberMaxLength = 50;
    
    /// <summary>
    /// Maximum length for application name.
    /// </summary>
    public const int ApplicationNameMaxLength = 200;
    
    /// <summary>
    /// Maximum length for application description.
    /// </summary>
    public const int ApplicationDescriptionMaxLength = 500;
}

/// <summary>
/// Domain-level urgency constants for incident management.
/// </summary>
public static class UrgencyLevels
{
    /// <summary>
    /// Minimum urgency level (highest priority).
    /// </summary>
    public const int Minimum = 1;
    
    /// <summary>
    /// Maximum urgency level (lowest priority).
    /// </summary>
    public const int Maximum = 3;
    
    /// <summary>
    /// Default urgency level.
    /// </summary>
    public const int Default = 3;
}