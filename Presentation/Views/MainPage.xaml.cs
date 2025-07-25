using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// A simple page that can be used on its own or navigated to within a Frame.
/// Follows MVVM pattern with proper ViewModel binding.
/// </summary>
public sealed partial class MainPage : Page
{
    /// <summary>
    /// Gets the ViewModel for this page.
    /// </summary>
    public MainPageViewModel ViewModel { get; }

    public MainPage()
    {
        // Create ViewModel - in production this would come from DI
        ViewModel = new MainPageViewModel();
        
        this.InitializeComponent();
        
        // Set DataContext for binding
        this.DataContext = ViewModel;
    }
}
