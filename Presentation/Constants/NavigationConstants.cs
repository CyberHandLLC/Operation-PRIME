namespace OperationPrime.Presentation.Constants;

/// <summary>
/// Constants for navigation view names to avoid magic strings and typos.
/// </summary>
public static class NavigationConstants
{
    /// <summary>
    /// Dashboard view - main overview page.
    /// </summary>
    public const string Dashboard = "Dashboard";

    /// <summary>
    /// Placeholder view - for unimplemented features.
    /// </summary>
    public const string Placeholder = "Placeholder";

    /// <summary>
    /// Incident creation view.
    /// </summary>
    public const string CreateIncident = "CreateIncident";

    /// <summary>
    /// Pre-Incident wizard view.
    /// </summary>
    public const string PreIncidentWizard = "PreIncidentWizard";

    /// <summary>
    /// Major Incident wizard view.
    /// </summary>
    public const string MajorIncidentWizard = "MajorIncidentWizard";

    /// <summary>
    /// Incident list view.
    /// </summary>
    public const string IncidentList = "IncidentList";

    /// <summary>
    /// Incident details view.
    /// </summary>
    public const string IncidentDetails = "IncidentDetails";

    /// <summary>
    /// Reports view.
    /// </summary>
    public const string Reports = "Reports";

    /// <summary>
    /// Settings view.
    /// </summary>
    public const string Settings = "Settings";

    /// <summary>
    /// Help view.
    /// </summary>
    public const string Help = "Help";

    /// <summary>
    /// Gets all available view names for validation.
    /// </summary>
    /// <returns>An array of all valid view names.</returns>
    public static string[] GetAllViewNames()
    {
        return new[]
        {
            Dashboard,
            Placeholder,
            CreateIncident,
            PreIncidentWizard,
            MajorIncidentWizard,
            IncidentList,
            IncidentDetails,
            Reports,
            Settings,
            Help
        };
    }

    /// <summary>
    /// Validates if a view name is valid.
    /// </summary>
    /// <param name="viewName">The view name to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool IsValidViewName(string viewName)
    {
        return GetAllViewNames().Contains(viewName, StringComparer.OrdinalIgnoreCase);
    }
}
