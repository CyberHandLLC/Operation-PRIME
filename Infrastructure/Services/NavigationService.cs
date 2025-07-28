using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;

namespace OperationPrime.Infrastructure.Services;

/// <summary>
/// Navigation service implementation for WinUI 3 applications
/// </summary>
public class NavigationService : INavigationService
{
    private Frame? _frame;
    private readonly Dictionary<string, Type> _pageRegistry = new();

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
            // Log the exception for debugging instead of silently failing
            System.Diagnostics.Debug.WriteLine($"Navigation to {pageType.Name} failed: {ex.Message}");
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
        System.Diagnostics.Debug.WriteLine($"Navigation failed: Page '{pageKey}' not found in registry");
        
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
    /// Clears the navigation history
    /// </summary>
    public void ClearHistory()
    {
        // Clear back stack if frame exists
        if (_frame != null)
        {
            _frame.BackStack.Clear();
        }
    }

    /// <summary>
    /// Register page types for string-based navigation
    /// This will be expanded as we add more pages
    /// </summary>
    private void RegisterPages()
    {
        // Register implemented pages
        _pageRegistry["IncidentList"] = typeof(OperationPrime.Presentation.Views.IncidentListView);
        
        // Register placeholder page for unimplemented features
        _pageRegistry["ComingSoon"] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        
        // Register placeholder pages for menu items that aren't implemented yet
        _pageRegistry["Dashboard"] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry["Reports"] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry["NOI"] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
        _pageRegistry["Settings"] = typeof(OperationPrime.Presentation.Views.ComingSoonView);
    }
}
