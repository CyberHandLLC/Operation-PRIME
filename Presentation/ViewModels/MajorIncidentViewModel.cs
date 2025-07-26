using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating and managing major incidents.
/// </summary>
public partial class MajorIncidentViewModel : IncidentViewModel
{
    private readonly INOIService _noiService;
    private readonly ILogger<MajorIncidentViewModel> _logger;

    public MajorIncidentViewModel(IIncidentService incidentService, INOIService noiService, ILogger<MajorIncidentViewModel> logger)
        : base(incidentService, logger)
    {
        _noiService = noiService;
        _logger = logger;
    }

    [ObservableProperty]
    [Required(ErrorMessage = "Incident commander is required")]
    private string incidentCommander = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Business impact is required")]
    private string businessImpact = string.Empty;

    [ObservableProperty]
    [Range(0, int.MaxValue)]
    private int affectedUsersCount;

    [ObservableProperty]
    private bool isCustomerFacing;

    [ObservableProperty]
    private string? bridgeDetails;

    [RelayCommand]
    private async Task SaveAsync()
    {
        _logger.LogDebug("Saving major incident {Number}", IncidentNumber);
        var entity = new MajorIncident
        {
            IncidentNumber = IncidentNumber,
            Title = Title,
            Description = Description,
            Status = Status,
            Priority = Priority,
            Urgency = Urgency,
            Impact = Impact,
            IncidentDateTime = IncidentDateTime,
            AffectedApplication = AffectedApplication,
            SupportTeam = SupportTeam,
            IncidentCommander = IncidentCommander,
            BusinessImpact = BusinessImpact,
            AffectedUsersCount = AffectedUsersCount,
            IsCustomerFacing = IsCustomerFacing,
            BridgeDetails = BridgeDetails
        };

        await IncidentService.CreateAsync(entity);
    }

    [RelayCommand]
    private string GenerateNoi(string templateType)
    {
        _logger.LogInformation("Generating NOI using template {Template}", templateType);
        var incident = new MajorIncident
        {
            IncidentNumber = IncidentNumber,
            Title = Title,
            Description = Description,
            Status = Status,
            Priority = Priority,
            Urgency = Urgency,
            Impact = Impact,
            IncidentDateTime = IncidentDateTime,
            AffectedApplication = AffectedApplication,
            SupportTeam = SupportTeam,
            IncidentCommander = IncidentCommander,
            BusinessImpact = BusinessImpact,
            AffectedUsersCount = AffectedUsersCount,
            IsCustomerFacing = IsCustomerFacing,
            BridgeDetails = BridgeDetails
        };

        return _noiService.GenerateNOI(incident, templateType);
    }
}

