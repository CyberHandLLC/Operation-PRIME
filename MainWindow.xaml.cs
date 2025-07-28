using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Presentation.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OperationPrime;

/// <summary>
/// Simple main application window for the incident management system.
/// </summary>
public sealed partial class MainWindow : Window
{
    public ShellViewModel ViewModel { get; }

    public MainWindow(ShellViewModel shellViewModel)
    {
        this.InitializeComponent();
        
        // Use constructor injection instead of service locator anti-pattern
        ViewModel = shellViewModel;
        
        // Set the navigation frame
        ViewModel.SetNavigationFrame(ContentFrame);
        
        // Set window title
        this.Title = "Operation Prime - Incident Management";
        
        // Subscribe to frame navigation events for breadcrumb updates
        ContentFrame.Navigated += ContentFrame_Navigated;
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ViewModel.ItemInvokedCommand.Execute(args);
    }

    private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        ViewModel.BackRequestedCommand.Execute(null);
    }

    /// <summary>
    /// Handles frame navigation to update breadcrumb context.
    /// </summary>
    private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        try
        {
            // Update page title and breadcrumb visibility based on current page
            if (e.Content is IncidentCreateView)
            {
                PageTitleText.Text = "Create New Incident";
                SetupIncidentCreateBreadcrumb();
                MainBreadcrumbBar.Visibility = Visibility.Visible;
            }
            else
            {
                PageTitleText.Text = "Operation Prime";
                MainBreadcrumbBar.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating breadcrumb context: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets up breadcrumb navigation for incident creation workflow.
    /// </summary>
    private void SetupIncidentCreateBreadcrumb()
    {
        try
        {
            var breadcrumbItems = new ObservableCollection<string>
            {
                "Type",
                "Basic Info",
                "Details",
                "Checklist"
            };
            
            MainBreadcrumbBar.ItemsSource = breadcrumbItems;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error setting up incident create breadcrumb: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles breadcrumb bar item clicks for step navigation.
    /// </summary>
    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        try
        {
            // Only handle breadcrumb navigation for incident creation page
            if (ContentFrame.Content is IncidentCreateView incidentCreateView)
            {
                var clickedIndex = args.Index;
                var targetStep = clickedIndex + 1;
                
                // Use the ViewModel's GoToStep command
                incidentCreateView.ViewModel.GoToStepCommand.Execute(targetStep);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error handling breadcrumb navigation: {ex.Message}");
        }
    }
}
