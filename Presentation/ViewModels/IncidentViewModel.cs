using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// Base ViewModel for all incident types. Provides common incident fields and validation.
/// </summary>
public partial class IncidentViewModel<TViewModel> : BaseViewModel
    where TViewModel : IncidentViewModel<TViewModel>
{
    protected readonly IIncidentService IncidentService;
    protected readonly ILogger<TViewModel> Logger;

    public IncidentViewModel(IIncidentService incidentService, ILogger<TViewModel> logger)
    {
        IncidentService = incidentService;
        Logger = logger;
    }

    [ObservableProperty]
    [Required(ErrorMessage = "Incident number is required")]
    private string incidentNumber = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
    private string title = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private IncidentStatus status = IncidentStatus.New;

    [ObservableProperty]
    private Priority priority = Priority.Medium;

    [ObservableProperty]
    private UrgencyLevel urgency = UrgencyLevel.Medium;

    [ObservableProperty]
    private ImpactLevel impact = ImpactLevel.Medium;

    [ObservableProperty]
    private DateTime incidentDateTime = DateTime.Now;

    [ObservableProperty]
    [Required(ErrorMessage = "Affected application is required")]
    private string affectedApplication = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Support team is required")]
    private string supportTeam = string.Empty;
}
