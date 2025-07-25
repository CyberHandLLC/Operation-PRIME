using Microsoft.UI.Xaml.Controls;
using OperationPrime.Application.Interfaces;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Placeholder view for features not yet implemented.
/// Provides user-friendly message and navigation back to Dashboard.
/// Follows MVVM pattern with proper ViewModel binding.
/// </summary>
public sealed partial class PlaceholderView : Page
{
    /// <summary>
    /// Gets the ViewModel for this page.
    /// </summary>
    public PlaceholderViewModel ViewModel { get; }

    public PlaceholderView()
    {
        this.InitializeComponent();
        
        // Create ViewModel - NavigationService will be injected later when DI is fully implemented
        // For now, we'll create a minimal version that doesn't use navigation
        ViewModel = CreatePlaceholderViewModel();
        
        // Set DataContext for binding
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Creates a PlaceholderViewModel with a mock NavigationService for now.
    /// TODO: Replace with proper DI when service layer is complete.
    /// </summary>
    private PlaceholderViewModel CreatePlaceholderViewModel()
    {
        // Create a mock navigation service for now
        var mockNavService = new MockNavigationService();
        return new PlaceholderViewModel(mockNavService);
    }

    /// <summary>
    /// Temporary mock navigation service until DI is fully implemented.
    /// </summary>
    private class MockNavigationService : INavigationService
    {
        public bool CanGoBack => false;
        public string? CurrentView => null;
        
#pragma warning disable CS0067 // Event is never used - this is a mock implementation
        public event EventHandler<NavigationEventArgs>? NavigationOccurred;
#pragma warning restore CS0067

        public bool NavigateTo(string viewName) => true; // Mock success
        public bool NavigateTo(string viewName, object parameter) => true; // Mock success
        public bool GoBack() => false; // Mock no-op
    }
}
