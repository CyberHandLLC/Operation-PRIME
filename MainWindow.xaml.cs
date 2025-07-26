using Microsoft.UI.Xaml;

namespace OperationPrime;

/// <summary>
/// Simple main application window for the incident management system.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        
        // Set window title
        this.Title = "Operation Prime - Incident Management";
    }


}
