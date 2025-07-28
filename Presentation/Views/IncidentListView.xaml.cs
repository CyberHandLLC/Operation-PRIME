using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Views;

public sealed partial class IncidentListView : Page
{
    /// <summary>
    /// Gets the ViewModel for this view.
    /// </summary>
    public IncidentListViewModel ViewModel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the IncidentListView.
    /// Uses parameterless constructor for WinUI 3 Frame navigation compatibility.
    /// ViewModel is resolved from DI container after initialization.
    /// </summary>
    public IncidentListView()
    {
        this.InitializeComponent();
        
        // Resolve ViewModel from DI container with error handling
        try
        {
            ViewModel = App.Current.GetService<IncidentListViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to resolve IncidentListViewModel: {ex.Message}");
            throw; // Re-throw as this is a critical failure
        }
        
        // Load incidents when page is loaded with proper exception handling
        this.Loaded += OnPageLoaded;
    }
    
    /// <summary>
    /// Handles the page loaded event and safely loads incidents
    /// </summary>
    private async void OnPageLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            await ViewModel.LoadIncidentsCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load incidents on page load: {ex.Message}");
            // The ViewModel should handle the error display, but we log it here for debugging
        }
    }
}
