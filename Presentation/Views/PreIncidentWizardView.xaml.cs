using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Presentation.ViewModels;
using OperationPrime;

namespace OperationPrime.Presentation.Views;

public sealed partial class PreIncidentWizardView : Page
{
    public PreIncidentWizardViewModel ViewModel { get; }

    public PreIncidentWizardView()
    {
        this.InitializeComponent();

        ViewModel = App.Current.GetService<PreIncidentWizardViewModel>();
        DataContext = ViewModel;
    }
}
