using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using OperationPrime.Presentation.ViewModels;
using OperationPrime;

namespace OperationPrime.Presentation.Views;

public sealed partial class MajorIncidentWizardView : Page
{
    public MajorIncidentWizardViewModel ViewModel { get; }

    public MajorIncidentWizardView()
    {
        this.InitializeComponent();

        ViewModel = App.Current.GetService<MajorIncidentWizardViewModel>();
        DataContext = ViewModel;
    }
}
