namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Interface for navigation service to abstract navigation logic from UI components.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="viewName">The name of the view to navigate to.</param>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    bool NavigateTo(string viewName);

    /// <summary>
    /// Navigates to the specified view with parameters.
    /// </summary>
    /// <param name="viewName">The name of the view to navigate to.</param>
    /// <param name="parameter">The parameter to pass to the view.</param>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    bool NavigateTo(string viewName, object parameter);

    /// <summary>
    /// Navigates back to the previous view if possible.
    /// </summary>
    /// <returns>True if navigation back was successful, false otherwise.</returns>
    bool GoBack();

    /// <summary>
    /// Gets a value indicating whether the navigation service can go back.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets the current view name.
    /// </summary>
    string? CurrentView { get; }

    /// <summary>
    /// Event raised when navigation occurs.
    /// </summary>
    event EventHandler<NavigationEventArgs>? NavigationOccurred;
}

/// <summary>
/// Event arguments for navigation events.
/// </summary>
public class NavigationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the name of the view being navigated to.
    /// </summary>
    public string ViewName { get; }

    /// <summary>
    /// Gets the parameter passed to the view, if any.
    /// </summary>
    public object? Parameter { get; }

    /// <summary>
    /// Gets the name of the previous view, if any.
    /// </summary>
    public string? PreviousView { get; }

    /// <summary>
    /// Initializes a new instance of the NavigationEventArgs class.
    /// </summary>
    /// <param name="viewName">The name of the view being navigated to.</param>
    /// <param name="parameter">The parameter passed to the view.</param>
    /// <param name="previousView">The name of the previous view.</param>
    public NavigationEventArgs(string viewName, object? parameter = null, string? previousView = null)
    {
        ViewName = viewName;
        Parameter = parameter;
        PreviousView = previousView;
    }
}
