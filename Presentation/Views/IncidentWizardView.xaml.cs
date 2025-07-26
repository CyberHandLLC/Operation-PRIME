using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Domain.Enums;
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is IncidentType type)
        {
            ViewModel.Initialize(type);
        }
    }
}
