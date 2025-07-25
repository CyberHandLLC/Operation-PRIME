using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Application.Interfaces;
using OperationPrime.Infrastructure.Services;
using OperationPrime.Presentation.Constants;
using OperationPrime.Presentation.Views;

namespace OperationPrime;

/// <summary>
/// Main application window with NavigationView shell following Clean Architecture UI patterns.
/// Implements navigation service and MVVM-compliant routing to page ViewModels.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow()
    {
        this.InitializeComponent();
        
        // Set window title
        this.Title = "OPERATION PRIME - Network Operations Center";
        
        // Initialize NavigationService with the ContentFrame
        _navigationService = new NavigationService(ContentFrame);
        
        // Register the NavigationService as singleton in DI (replace placeholder)
        var services = App.Current.Services as ServiceCollection;
        // Note: In production, we'd configure this differently, but for now this works
        
        // Navigate to Dashboard by default
        _navigationService.NavigateTo(NavigationConstants.Dashboard);
    }

    /// <summary>
    /// Handles NavigationView selection changes and routes to appropriate pages
    /// </summary>
    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem selectedItem)
        {
            var tag = selectedItem.Tag?.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                NavigateToPage(tag);
            }
        }
    }

    /// <summary>
    /// Navigation routing method using NavigationService with constants
    /// Following Clean Architecture pattern - UI routes to Presentation layer Views
    /// </summary>
    private void NavigateToPage(string pageTag)
    {
        // Map legacy tags to NavigationConstants
        var viewName = pageTag switch
        {
            "Dashboard" => NavigationConstants.Dashboard,
            "CreatePreIncident" => NavigationConstants.CreateIncident,
            "CreateMajorIncident" => NavigationConstants.CreateIncident,
            "ViewIncidents" => NavigationConstants.IncidentList,
            "Reports" => NavigationConstants.Reports,
            "Settings" => NavigationConstants.Settings,
            "Help" => NavigationConstants.Help,
            _ => NavigationConstants.Dashboard // Default fallback
        };

        try
        {
            var success = _navigationService.NavigateTo(viewName);
            if (!success)
            {
                // Fallback to Dashboard if navigation fails
                _navigationService.NavigateTo(NavigationConstants.Dashboard);
                System.Diagnostics.Debug.WriteLine($"Navigation failed for {pageTag}, falling back to Dashboard");
            }
        }
        catch (System.Exception ex)
        {
            // Fallback to Dashboard on any exception
            _navigationService.NavigateTo(NavigationConstants.Dashboard);
            System.Diagnostics.Debug.WriteLine($"Navigation exception for {pageTag}: {ex.Message}");
        }
    }

    /// <summary>
    /// Public method for programmatic navigation using NavigationService
    /// </summary>
    public bool NavigateTo(string viewName)
    {
        try
        {
            return _navigationService.NavigateTo(viewName);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the navigation service for external access.
    /// </summary>
    public INavigationService NavigationService => _navigationService;
}
