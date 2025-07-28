using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Placeholder page for features under development
/// </summary>
public sealed partial class ComingSoonView : Page
{
    public ComingSoonView()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // If a parameter was passed, use it to customize the message
        if (e.Parameter is string pageName && PageNameTextBlock != null)
        {
            PageNameTextBlock.Text = $"{pageName} is under development";
        }
    }
}
