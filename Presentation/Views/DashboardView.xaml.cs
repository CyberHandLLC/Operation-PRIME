using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Presentation.ViewModels;
using OperationPrime;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Dashboard view following Clean Architecture UI patterns.
/// Displays key metrics, recent incidents, and quick actions for network operations personnel.
/// </summary>
public sealed partial class DashboardView : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardView()
    {
        this.InitializeComponent();

        ViewModel = App.Current.GetService<DashboardViewModel>();
        DataContext = ViewModel;
    }
}
