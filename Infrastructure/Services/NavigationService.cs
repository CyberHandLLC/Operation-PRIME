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
        catch
        {
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
        // TODO: Register pages as they are created
        // Example: _pageRegistry["IncidentList"] = typeof(IncidentListPage);
        // Example: _pageRegistry["Dashboard"] = typeof(DashboardPage);
    }
}
