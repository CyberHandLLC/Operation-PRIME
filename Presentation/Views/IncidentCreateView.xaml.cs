using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Presentation.ViewModels;
using System.Diagnostics;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Page for creating new incidents.
/// Follows MVVM pattern with proper ViewModel binding and error handling.
/// </summary>
public sealed partial class IncidentCreateView : Page
{
    /// <summary>
    /// Gets the ViewModel for this view.
    /// </summary>
    public IncidentCreateViewModel ViewModel { get; private set; }

    /// <summary>
    /// Initializes a new instance of the IncidentCreateView.
    /// </summary>
    public IncidentCreateView()
    {
        this.InitializeComponent();
        
        try
        {
            // Get ViewModel from DI container
            ViewModel = App.Current.GetService<IncidentCreateViewModel>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to resolve IncidentCreateViewModel: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Handles navigation to this page.
    /// </summary>
    /// <param name="e">Navigation event arguments.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        try
        {
            // Reset form when navigating to the page
            ViewModel.ResetFormCommand.Execute(null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during navigation: {ex.Message}");
        }
    }

}
