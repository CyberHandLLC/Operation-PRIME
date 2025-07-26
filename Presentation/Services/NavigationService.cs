using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Constants;
using OperationPrime.Presentation.Views;

namespace OperationPrime.Presentation.Services;

/// <summary>
/// Implementation of navigation service for WinUI 3 applications.
/// Resides in Presentation layer to avoid Clean Architecture violations.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Frame _frame;
    private readonly Stack<string> _navigationHistory;
    private readonly ILogger<NavigationService> _logger;

    /// <summary>
    /// Initializes a new instance of the NavigationService class.
    /// </summary>
    /// <param name="frame">The frame to use for navigation.</param>
    public NavigationService(Frame frame, ILogger<NavigationService> logger)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        _navigationHistory = new Stack<string>();
        _logger = logger;
        _logger.LogDebug("NavigationService initialized");
    }

    /// <summary>
    /// Gets a value indicating whether the navigation service can go back.
    /// </summary>
    public bool CanGoBack => _frame.CanGoBack;

    /// <summary>
    /// Gets the current view name.
    /// </summary>
    public string? CurrentView { get; private set; }

    /// <summary>
    /// Event raised when navigation occurs.
    /// </summary>
    public event EventHandler<NavigationEventArgs>? NavigationOccurred;

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="viewName">The name of the view to navigate to.</param>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    public bool NavigateTo(string viewName)
    {
        return NavigateTo(viewName, null);
    }

    /// <summary>
    /// Navigates to the specified view with parameters.
    /// </summary>
    /// <param name="viewName">The name of the view to navigate to.</param>
    /// <param name="parameter">The parameter to pass to the view.</param>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    public bool NavigateTo(string viewName, object? parameter)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            return false;

        _logger.LogDebug("Navigating to {View} with parameter {Parameter}", viewName, parameter);

        // Validate view name
        if (!NavigationConstants.IsValidViewName(viewName))
            return false;

        // Get the target page type
        var pageType = GetPageTypeForView(viewName);
        if (pageType == null)
            return false;

        try
        {
            // Store previous view for history
            var previousView = CurrentView;
            if (!string.IsNullOrEmpty(CurrentView))
            {
                _navigationHistory.Push(CurrentView);
            }

            // Perform navigation
            var success = _frame.Navigate(pageType, parameter);
            
            if (success)
            {
                CurrentView = viewName;
                _logger.LogDebug("Navigation to {View} succeeded", viewName);

                // Raise navigation event
                NavigationOccurred?.Invoke(this, new NavigationEventArgs(viewName, parameter, previousView));
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Navigation to {View} failed", viewName);
            return false;
        }
    }

    /// <summary>
    /// Navigates back to the previous view if possible.
    /// </summary>
    /// <returns>True if navigation back was successful, false otherwise.</returns>
    public bool GoBack()
    {
        if (!CanGoBack)
            return false;

        _logger.LogDebug("Navigating back from {View}", CurrentView);

        try
        {
            var previousView = CurrentView;
            _frame.GoBack();

            // Update current view from history
            if (_navigationHistory.Count > 0)
            {
                CurrentView = _navigationHistory.Pop();
            }
            else
            {
                CurrentView = null;
            }

            _logger.LogDebug("Navigated back to {View}", CurrentView);

            // Raise navigation event
            NavigationOccurred?.Invoke(this, new NavigationEventArgs(CurrentView ?? "Unknown", null, previousView));

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GoBack failed");
            return false;
        }
    }

    /// <summary>
    /// Gets the page type for the specified view name.
    /// </summary>
    /// <param name="viewName">The view name.</param>
    /// <returns>The page type, or null if not found.</returns>
    private static Type? GetPageTypeForView(string viewName)
    {
        return viewName switch
        {
            NavigationConstants.Dashboard => typeof(DashboardView),
            NavigationConstants.Placeholder => typeof(PlaceholderView),
            NavigationConstants.CreateIncident => typeof(PlaceholderView), // TODO: Replace with actual view
            NavigationConstants.IncidentList => typeof(PlaceholderView), // TODO: Replace with actual view
            NavigationConstants.IncidentDetails => typeof(PlaceholderView), // TODO: Replace with actual view
            NavigationConstants.Reports => typeof(PlaceholderView), // TODO: Replace with actual view
            NavigationConstants.Settings => typeof(PlaceholderView), // TODO: Replace with actual view
            NavigationConstants.Help => typeof(PlaceholderView), // TODO: Replace with actual view
            _ => null
        };
    }
}
