using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Views;

public sealed partial class IncidentListView : Page
{
    /// <summary>
    /// Gets the ViewModel for this view.
    /// </summary>
    public IncidentListViewModel ViewModel { get; private set; }
    
    private readonly ILogger<IncidentListView> _logger;

    /// <summary>
    /// Initializes a new instance of the IncidentListView.
    /// Uses parameterless constructor for WinUI 3 Frame navigation compatibility.
    /// ViewModel is resolved from DI container after initialization.
    /// </summary>
    public IncidentListView()
    {
        this.InitializeComponent();
        
        // Resolve dependencies from DI container following established patterns
        // Uses service locator pattern consistent with IncidentCreateView
        ViewModel = App.Current.Services.GetRequiredService<IncidentListViewModel>();
        _logger = App.Current.Services.GetRequiredService<ILogger<IncidentListView>>();
        
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
            _logger.LogError(ex, "Failed to load incidents on page load");
            // The ViewModel should handle the error display, but we log it here for debugging
        }
    }
}
