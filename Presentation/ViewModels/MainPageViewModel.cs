using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for the MainPage view.
/// </summary>
public partial class MainPageViewModel : BaseViewModel
{
    private readonly ILogger<MainPageViewModel> _logger;
    /// <summary>
    /// Initializes a new instance of the MainPageViewModel class.
    /// </summary>
    public MainPageViewModel(ILogger<MainPageViewModel> logger)
    {
        _logger = logger;
        Title = "Main Page";
        CountText = "Current count: 0";
        _logger.LogDebug("MainPageViewModel initialized");
    }

    /// <summary>
    /// Gets or sets the current count value.
    /// </summary>
    [ObservableProperty]
    public partial int Count { get; set; } = 0;

    /// <summary>
    /// Gets or sets the count display text.
    /// </summary>
    [ObservableProperty]
    public partial string CountText { get; set; } = string.Empty;

    /// <summary>
    /// Command to increment the count.
    /// </summary>
    [RelayCommand]
    private void IncrementCount()
    {
        Count++;
        CountText = $"Current count: {Count}";
        _logger.LogDebug("Count incremented to {Count}", Count);
    }
}
