using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.Constants;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Navigation service implementation for WinUI 3 applications
/// </summary>
public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private Frame? _frame;
    private readonly Dictionary<string, Type> _pageRegistry = new();

    /// <summary>
    /// Initializes a new instance of the NavigationService.
    /// </summary>
    /// <param name="logger">Logger for structured logging.</param>
    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the current navigation frame
    /// </summary>
    public Frame? Frame => _frame;

    /// <summary>
    /// Gets whether the frame can navigate back
    /// </summary>
    public bool CanGoBack => _frame?.CanGoBack ?? false;

    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    public event EventHandler<string>? NavigationChanged;

    /// <summary>
    /// Sets the navigation frame
    /// </summary>
    /// <param name="frame">The frame to use for navigation</param>
    public void SetFrame(Frame frame)
    {
        _frame = frame;
        RegisterPages();
    }

    /// <summary>
    /// Navigates to the specified page type
    /// </summary>
    /// <param name="pageType">The type of page to navigate to</param>
    /// <param name="parameter">Optional parameter to pass to the page</param>
    /// <returns>True if navigation was successful</returns>
    public bool NavigateTo(Type pageType, object? parameter = null)
    {
        if (_frame == null)
            return false;

        try
        {
            var result = _frame.Navigate(pageType, parameter);
            if (result)
            {
                NavigationChanged?.Invoke(this, pageType.Name);
            }
            return result;
        }
        catch (Exception ex)
        {
            // Use structured logging instead of Debug.WriteLine
            _logger.LogError(ex, "Navigation to {PageType} failed", pageType.Name);
            return false;
        }
    }

    /// <summary>
    /// Navigates to the specified page type by name
    /// </summary>
    /// <param name="pageKey">The key/name of the page to navigate to</param>
    /// <param name="parameter">Optional parameter to pass to the page</param>
    /// <returns>True if navigation was successful</returns>
    public bool NavigateTo(string pageKey, object? parameter = null)
    {
        if (_pageRegistry.TryGetValue(pageKey, out var pageType))
        {
            return NavigateTo(pageType, parameter);
        }
        
        // Handle missing pages gracefully - navigate to placeholder or show message
        _logger.LogWarning("Navigation failed: Page '{PageKey}' not found in registry", pageKey);
        
        // For now, return false to indicate navigation failure
        // In the future, we could navigate to a "Page Not Found" or "Coming Soon" page
        return false;
    }

    /// <summary>
    /// Navigates back if possible
    /// </summary>
    /// <returns>True if navigation back was successful</returns>
    public bool GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
            NavigationChanged?.Invoke(this, "Back");
            return true;
        }
        return false;
    }



    /// <summary>
    /// Register page types for string-based navigation
    /// This will be expanded as we add more pages
    /// </summary>
    private void RegisterPages()
    {
        // Register implemented pages using NavigationKeys constants
        _pageRegistry[NavigationKeys.Incidents] = typeof(OperationPrime.Presentation.Views.IncidentListView);
        _pageRegistry[NavigationKeys.IncidentCreate] = typeof(OperationPrime.Presentation.Views.IncidentCreateView);
        
        // Register placeholder page for unimplemented features
        _pageRegistry[NavigationKeys.ComingSoon] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        
        // Register placeholder pages for menu items that aren't implemented yet
        _pageRegistry[NavigationKeys.Dashboard] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry[NavigationKeys.Reports] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry[NavigationKeys.NOI] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry[NavigationKeys.Settings] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        
        _logger.LogDebug("Registered {PageCount} navigation pages", _pageRegistry.Count);
    }
}
