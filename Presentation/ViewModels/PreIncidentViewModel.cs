using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Entities;

namespace OperationPrime.Presentation.ViewModels;

/// <summary>
/// ViewModel for creating and managing pre-incidents.
/// </summary>
public partial class PreIncidentViewModel : IncidentViewModel<PreIncidentViewModel>
{
    public PreIncidentViewModel(IIncidentService incidentService, ILogger<PreIncidentViewModel> logger)
        : base(incidentService, logger)
    {
    }

    [ObservableProperty]
    [Required(ErrorMessage = "Identified by is required")]
    private string identifiedBy = string.Empty;

    [ObservableProperty]
    private string? potentialImpact;

    [RelayCommand]
    private async Task SaveAsync()
    {
        Logger.LogDebug("Saving pre-incident {Number}", IncidentNumber);
        var entity = new PreIncident
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
            IdentifiedBy = IdentifiedBy,
            PotentialImpact = PotentialImpact
        };

        await IncidentService.CreateAsync(entity);
    }
}

