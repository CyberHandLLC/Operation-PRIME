using OperationPrime.Application.DTOs;
using OperationPrime.Presentation.ViewModels;

namespace OperationPrime.Presentation.Extensions;

/// <summary>
/// Extension methods for ViewModels to eliminate repetitive DTO mapping.
/// Follows DRY principles while maintaining Clean Architecture boundaries.
/// </summary>
public static class ViewModelExtensions
{
    /// <summary>
    /// Converts IncidentCreateViewModel to IncidentFormData DTO.
    /// Centralizes mapping logic to eliminate repetition across the ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel to convert</param>
    /// <returns>IncidentFormData with all properties mapped</returns>
    public static IncidentFormData ToFormData(this IncidentCreateViewModel viewModel)
    {
        return new IncidentFormData
        {
            Title = viewModel.Title,
            Description = viewModel.Description,
            BusinessImpact = viewModel.BusinessImpact,
            TimeIssueStarted = viewModel.TimeIssueStarted,
            TimeReported = viewModel.TimeReported,
            ImpactedUsers = viewModel.ImpactedUsers,
            ApplicationAffected = viewModel.ApplicationAffected,
            LocationsAffected = viewModel.LocationsAffected,
            Workaround = viewModel.Workaround,
            IncidentNumber = viewModel.IncidentNumber,
            Urgency = viewModel.Urgency,
            IncidentType = viewModel.IncidentType,
            Priority = viewModel.Priority,
            Status = viewModel.Status,
            SelectedImpactedUsersCount = viewModel.SelectedImpactedUsersCount,
            CurrentStep = viewModel.CurrentStep,
            IsSubmitting = viewModel.IsSubmitting,
            ErrorMessage = viewModel.ErrorMessage,
            SuccessMessage = viewModel.SuccessMessage
        };
    }



    /// <summary>
    /// Loads a collection using a generic pattern to eliminate repetitive foreach loops.
    /// Follows DRY principles for ObservableCollection population.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The collection to populate</param>
    /// <param name="source">The source data to load from</param>
    public static void LoadFrom<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, IEnumerable<T> source)
    {
        collection.Clear();
        foreach (var item in source)
        {
            collection.Add(item);
        }
    }
} 