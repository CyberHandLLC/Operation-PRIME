using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OperationPrime.Presentation.ViewModels;

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
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ViewModel.ItemInvokedCommand.Execute(args);
    }

    private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        ViewModel.BackRequestedCommand.Execute(null);
    }
}
