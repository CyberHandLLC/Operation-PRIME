namespace OperationPrime.Presentation.Constants;

/// <summary>
/// Centralized navigation page keys to replace magic strings.
/// Follows Microsoft maintainability best practices.
/// </summary>
public static class NavigationKeys
{
    /// <summary>
    /// Dashboard page key.
    /// </summary>
    public const string Dashboard = "Dashboard";

    /// <summary>
    /// Incidents list page key.
    /// </summary>
    public const string Incidents = "Incidents";

    /// <summary>
    /// Reports page key.
    /// </summary>
    public const string Reports = "Reports";

    /// <summary>
    /// NOI (Notice of Incident) page key.
    /// </summary>
    public const string NOI = "NOI";

    /// <summary>
    /// Settings page key.
    /// </summary>
    public const string Settings = "Settings";

    /// <summary>
    /// Coming Soon placeholder page key.
    /// </summary>
    public const string ComingSoon = "ComingSoon";

    /// <summary>
    /// Incident Create page key.
    /// </summary>
    public const string IncidentCreate = "IncidentCreate";

    /// <summary>
    /// Gets a friendly display name for a navigation key.
    /// </summary>
    /// <param name="navigationKey">The navigation key.</param>
    /// <returns>A user-friendly display name.</returns>
    public static string GetFriendlyName(string navigationKey) => navigationKey switch
    {
        Dashboard => "Dashboard",
        Incidents => "Incident Management",
        Reports => "Reports",
        NOI => "Notice of Incident",
        Settings => "Settings",
        ComingSoon => "Coming Soon",
        IncidentCreate => "Create New Incident",
        _ => navigationKey
    };

    /// <summary>
    /// Gets all available navigation keys.
    /// </summary>
    public static readonly string[] AllKeys = 
    {
        Dashboard,
        Incidents,
        Reports,
        NOI,
        Settings
    };
}
