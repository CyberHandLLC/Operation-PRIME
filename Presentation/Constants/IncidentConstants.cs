namespace OperationPrime.Presentation.Constants;

/// <summary>
/// Constants related to incident management and UI.
/// Centralizes hardcoded values for better maintainability.
/// </summary>
public static class IncidentConstants
{
    /// <summary>
    /// Format string for generating incident numbers.
    /// Pattern: INC-YYYYMMDD-####
    /// </summary>
    public const string IncidentNumberFormat = "INC-{0:yyyyMMdd}-{1:D4}";
    
    /// <summary>
    /// Prefix for incident numbers.
    /// </summary>
    public const string IncidentNumberPrefix = "INC-";
    
    /// <summary>
    /// Date format for incident numbers.
    /// </summary>
    public const string IncidentNumberDateFormat = "yyyyMMdd";
    
    /// <summary>
    /// Modulo value for generating unique incident sequence numbers.
    /// </summary>
    public const int IncidentSequenceModulo = 10000;
    
    /// <summary>
    /// Default minimum height for multi-line text inputs.
    /// </summary>
    public const int MultiLineTextMinHeight = 120;
    
    /// <summary>
    /// Default maximum height for multi-line text inputs.
    /// </summary>
    public const int MultiLineTextMaxHeight = 200;
}

/// <summary>
/// Validation length constants for incident fields.
/// Centralized to ensure consistency across Domain, ViewModel, and Database layers.
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
/// UI placeholder text constants for consistent user experience.
/// </summary>
public static class PlaceholderText
{
    public const string IncidentTitle = "Clear, concise incident summary";
    public const string IncidentDescription = "Detailed explanation of the issue";
    public const string BusinessImpact = "Describe what users cannot do due to this incident";
    public const string ApplicationAffected = "Start typing to search applications...";
    public const string LocationsAffected = "Geographic or organizational areas affected";
    public const string Workaround = "Any temporary solutions or workarounds available";
    public const string IncidentNumber = "Leave empty for auto-generation";
    public const string ImpactedUsers = "Select number of impacted users";
} 