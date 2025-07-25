using Microsoft.UI.Xaml.Controls;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Dashboard view following Clean Architecture UI patterns.
/// Displays key metrics, recent incidents, and quick actions for network operations personnel.
/// </summary>
public sealed partial class DashboardView : Page
{
    public DashboardView()
    {
        this.InitializeComponent();
        
        // TODO: Bind to DashboardViewModel when implemented
        // TODO: Load dashboard data through Application layer services
    }
}
