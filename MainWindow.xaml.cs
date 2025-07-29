using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Presentation.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace OperationPrime;

/// <summary>
/// Simple main application window for the incident management system.
/// </summary>
public sealed partial class MainWindow : Window
{
    public ShellViewModel ViewModel { get; }
    private IncidentCreateViewModel? _currentIncidentViewModel;
    private bool _disposed = false;

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
        
        // Subscribe to window closed event for cleanup
        this.Closed += MainWindow_Closed;
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
                
                // Unsubscribe from ViewModel when leaving incident creation page
                if (_currentIncidentViewModel != null)
                {
                    _currentIncidentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _currentIncidentViewModel = null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating breadcrumb context: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets up breadcrumb navigation for incident creation workflow.
    /// Dynamically shows 3 steps for Pre-Incident, 4 steps for Major Incident.
    /// </summary>
    private void SetupIncidentCreateBreadcrumb()
    {
        try
        {
            // Get the current incident creation view and its ViewModel
            if (ContentFrame.Content is IncidentCreateView incidentCreateView)
            {
                var viewModel = incidentCreateView.ViewModel;
                
                // Create breadcrumb items based on incident type
                var breadcrumbItems = new ObservableCollection<string>
                {
                    "Type",
                    "Basic Info",
                    "Details"
                };
                
                // Add Checklist step only for Major Incidents
                if (viewModel.IsMajorIncident)
                {
                    breadcrumbItems.Add("Checklist");
                }
                
                MainBreadcrumbBar.ItemsSource = breadcrumbItems;
                
                // Unsubscribe from previous ViewModel if exists
                if (_currentIncidentViewModel != null)
                {
                    _currentIncidentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                
                // Subscribe to IncidentType changes to update breadcrumb dynamically
                _currentIncidentViewModel = viewModel;
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
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

    /// <summary>
    /// Handles ViewModel property changes to update breadcrumb dynamically.
    /// </summary>
    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            // Update breadcrumb when IncidentType changes
            if (e.PropertyName == nameof(IncidentCreateViewModel.IncidentType) || 
                e.PropertyName == nameof(IncidentCreateViewModel.IsMajorIncident))
            {
                SetupIncidentCreateBreadcrumb();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error handling ViewModel property change: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles window closed event to perform cleanup and prevent memory leaks.
    /// </summary>
    private void MainWindow_Closed(object sender, WindowEventArgs e)
    {
        DisposeResources();
    }

    /// <summary>
    /// Disposes of resources and unsubscribes from events to prevent memory leaks.
    /// </summary>
    private void DisposeResources()
    {
        if (_disposed)
            return;

        try
        {
            // Unsubscribe from frame navigation events
            ContentFrame.Navigated -= ContentFrame_Navigated;
            
            // Unsubscribe from window events
            this.Closed -= MainWindow_Closed;
            
            // Unsubscribe from current incident ViewModel if exists
            if (_currentIncidentViewModel != null)
            {
                _currentIncidentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _currentIncidentViewModel = null;
            }
            
            // Dispose ViewModel if it implements IDisposable
            if (ViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
            
            _disposed = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during MainWindow disposal: {ex.Message}");
        }
    }
}
