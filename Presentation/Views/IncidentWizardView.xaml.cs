using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Presentation.ViewModels;
using OperationPrime;

namespace OperationPrime.Presentation.Views;

public sealed partial class IncidentWizardView : Page
{
    public IncidentWizardViewModel ViewModel { get; }

    public IncidentWizardView()
    {
        this.InitializeComponent();

        ViewModel = App.Current.GetService<IncidentWizardViewModel>();
        DataContext = ViewModel;
    }
}
