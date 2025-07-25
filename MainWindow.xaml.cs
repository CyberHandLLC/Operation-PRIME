using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Presentation.Views;

namespace OperationPrime;

/// <summary>
/// Main application window with NavigationView shell following Clean Architecture UI patterns.
/// Implements navigation service and MVVM-compliant routing to page ViewModels.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        
        // Set window title
        this.Title = "OPERATION PRIME - Network Operations Center";
        
        // Navigate to Dashboard by default
        NavigateToPage("Dashboard");
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
    /// Navigation routing method that maps tags to page types
    /// Following Clean Architecture pattern - UI routes to Presentation layer Views
    /// </summary>
    private void NavigateToPage(string pageTag)
    {
        Type? pageType = pageTag switch
        {
            "Dashboard" => typeof(DashboardView),
            // TODO: Create these Views in Presentation/Views/ as we implement features
            "CreatePreIncident" => typeof(PlaceholderView), // Will create CreatePreIncidentView later
            "CreateMajorIncident" => typeof(PlaceholderView), // Will create CreateMajorIncidentView later
            "ViewIncidents" => typeof(PlaceholderView), // Will create IncidentListView later
            "Reports" => typeof(PlaceholderView), // Will create ReportsView later
            _ => typeof(DashboardView) // Default fallback to Dashboard
        };

        if (pageType != null)
        {
            try
            {
                ContentFrame.Navigate(pageType);
            }
            catch (System.Exception ex)
            {
                // For now, fallback to MainPage if navigation fails
                // TODO: Implement proper error handling and logging
                ContentFrame.Navigate(typeof(MainPage));
                System.Diagnostics.Debug.WriteLine($"Navigation failed for {pageTag}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Public method for programmatic navigation (for future navigation service)
    /// </summary>
    public bool NavigateTo(string pageTag)
    {
        try
        {
            NavigateToPage(pageTag);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
