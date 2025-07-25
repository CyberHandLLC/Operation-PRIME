using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Placeholder view for features not yet implemented.
/// Provides user-friendly message and navigation back to Dashboard.
/// </summary>
public sealed partial class PlaceholderView : Page
{
    public PlaceholderView()
    {
        this.InitializeComponent();
    }

    private void BackToDashboard_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back to Dashboard
        if (this.Frame != null)
        {
            this.Frame.Navigate(typeof(DashboardView));
        }
    }
}
