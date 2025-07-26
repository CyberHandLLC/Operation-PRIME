using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.Logging.Abstractions;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Views;

public sealed partial class IncidentWizardView : Page
{
    public IncidentWizardViewModel ViewModel { get; }

    public IncidentWizardView()
    {
        this.InitializeComponent();

        // Temporary ViewModel creation until DI is wired for pages
        var logger = NullLogger<IncidentWizardViewModel>.Instance;
        ViewModel = new IncidentWizardViewModel(logger);
        DataContext = ViewModel;
    }
}
