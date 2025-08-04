using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OperationPrime.Presentation.ViewModels;
using OperationPrime.Domain.Entities;
using System.Diagnostics;
using System.Linq;

namespace OperationPrime.Presentation.Views;

/// <summary>
/// Page for creating new incidents.
/// Follows MVVM pattern with proper ViewModel binding and error handling.
/// </summary>
public sealed partial class IncidentCreateView : Page
{
    /// <summary>
    /// Gets the ViewModel for this view.
    /// </summary>
    public IncidentCreateViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the IncidentCreateView.
    /// Uses service locator pattern to resolve ViewModel from DI container.
    /// This approach is necessary for WinUI 3 Frame navigation which uses reflection.
    /// </summary>
    public IncidentCreateView()
    {
        // Resolve ViewModel from DI container using service locator pattern
        // This is the recommended approach for WinUI 3 pages with Frame navigation
        ViewModel = App.Current.Services.GetRequiredService<IncidentCreateViewModel>();
        
        this.InitializeComponent();
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Handles navigation to this page.
    /// Supports navigation parameters for editing existing incidents or loading templates.
    /// </summary>
    /// <param name="e">Navigation event arguments.</param>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        try
        {
            // Handle navigation parameters for editing or templates
            await HandleNavigationParametersAsync(e.Parameter);
            
            // Reset form when navigating to the page (if not editing)
            if (e.Parameter == null)
            {
                ViewModel.ResetFormCommand.Execute(null);
            }
            
            // Initialize DatePicker and TimePicker controls with current values
            InitializeDateTimeControls();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during navigation: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Handles navigation parameters for editing existing incidents or loading templates.
    /// </summary>
    /// <param name="parameter">Navigation parameter (incident ID, template, etc.)</param>
    private async Task HandleNavigationParametersAsync(object? parameter)
    {
        // TODO: Future enhancement - handle navigation parameters
        // Examples:
        // - if (parameter is string incidentId) await ViewModel.LoadIncidentForEditingAsync(incidentId);
        // - if (parameter is IncidentTemplate template) await ViewModel.LoadFromTemplateAsync(template);
        // - await ViewModel.InitializeDataAsync(); // Preload dropdown data
        
        await Task.CompletedTask; // Placeholder to satisfy async requirement
    }

    /// <summary>
    /// Handles date selection changes for Time Issue Started.
    /// </summary>
    private void TimeIssueStartedDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
        UpdateTimeIssueStarted();
    }

    /// <summary>
    /// Handles time selection changes for Time Issue Started.
    /// </summary>
    private void TimeIssueStartedTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
    {
        UpdateTimeIssueStarted();
    }

    /// <summary>
    /// Notifies ViewModel of date/time changes for Time Issue Started.
    /// UI layer only passes data - business logic handled by ViewModel/Services.
    /// </summary>
    private void UpdateTimeIssueStarted()
    {
        if (TimeIssueStartedDatePicker.SelectedDate != null && TimeIssueStartedTimePicker.SelectedTime != null)
        {
            // ✅ CLEAN: UI only passes data, no business logic
            ViewModel.UpdateTimeIssueStarted(
                TimeIssueStartedDatePicker.SelectedDate.Value,
                TimeIssueStartedTimePicker.SelectedTime.Value);
        }
        else if (TimeIssueStartedDatePicker.SelectedDate == null && TimeIssueStartedTimePicker.SelectedTime == null)
        {
            ViewModel.ClearTimeIssueStarted();
        }
    }

    /// <summary>
    /// Handles date selection changes for Time Reported.
    /// </summary>
    private void TimeReportedDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
        UpdateTimeReported();
    }

    /// <summary>
    /// Handles time selection changes for Time Reported.
    /// </summary>
    private void TimeReportedTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
    {
        UpdateTimeReported();
    }

    /// <summary>
    /// Notifies ViewModel of date/time changes for Time Reported.
    /// UI layer only passes data - business logic handled by ViewModel/Services.
    /// </summary>
    private void UpdateTimeReported()
    {
        if (TimeReportedDatePicker.SelectedDate != null && TimeReportedTimePicker.SelectedTime != null)
        {
            // ✅ CLEAN: UI only passes data, no business logic
            ViewModel.UpdateTimeReported(
                TimeReportedDatePicker.SelectedDate.Value,
                TimeReportedTimePicker.SelectedTime.Value);
        }
        else if (TimeReportedDatePicker.SelectedDate == null && TimeReportedTimePicker.SelectedTime == null)
        {
            ViewModel.ClearTimeReported();
        }
    }

    /// <summary>
    /// Initializes DatePicker and TimePicker controls with current values from ViewModel.
    /// This ensures the controls show the prefilled values and validation works correctly.
    /// </summary>
    private void InitializeDateTimeControls()
    {
        try
        {
            // Initialize TimeIssueStarted controls with ViewModel Eastern Time
            if (ViewModel.TimeIssueStarted.HasValue)
            {
                var easternTime = ViewModel.TimeIssueStarted.Value;
                TimeIssueStartedDatePicker.SelectedDate = easternTime;
                TimeIssueStartedTimePicker.SelectedTime = easternTime.TimeOfDay;
                
                Debug.WriteLine($"UI: Set TimeIssueStarted to {easternTime} (offset: {easternTime.Offset})");
            }
            
            // Initialize TimeReported controls with ViewModel Eastern Time
            if (ViewModel.TimeReported.HasValue)
            {
                var easternTime = ViewModel.TimeReported.Value;
                TimeReportedDatePicker.SelectedDate = easternTime;
                TimeReportedTimePicker.SelectedTime = easternTime.TimeOfDay;
                
                Debug.WriteLine($"UI: Set TimeReported to {easternTime} (offset: {easternTime.Offset})");
            }
            
            // ✅ CLEAN: ViewModel handles validation automatically via property change notifications
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing date/time controls: {ex.Message}");
        }
    }

    /// <summary>
    /// Public method to refresh date/time controls after ViewModel changes.
    /// Can be called when the form is reset or values are updated programmatically.
    /// </summary>
    public void RefreshDateTimeControls()
    {
        // Force reset the UI controls to current ViewModel values
        // This is needed when ViewModel is reset or updated programmatically
        InitializeDateTimeControls();
    }
    

    
    /// <summary>
    /// Handles text changes in the Application AutoSuggestBox to filter suggestions.
    /// Follows Microsoft WinUI 3 AutoSuggestBox documentation patterns.
    /// </summary>
    /// <param name="sender">The AutoSuggestBox that triggered the event.</param>
    /// <param name="args">Event arguments containing the query text.</param>
    private void ApplicationAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        try
        {
            // Only filter when the user is typing (not when programmatically setting text)
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var query = sender.Text?.Trim();
                
                if (string.IsNullOrEmpty(query))
                {
                    // Show all applications when query is empty
                    sender.ItemsSource = ViewModel.AvailableApplications;
                }
                else
                {
                    // Filter applications based on name (case-insensitive)
                    var filteredApps = ViewModel.AvailableApplications
                        .Where(app => app.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    sender.ItemsSource = filteredApps;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error filtering applications: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Handles suggestion selection in the Application AutoSuggestBox.
    /// Updates the ViewModel property when user selects an application.
    /// </summary>
    /// <param name="sender">The AutoSuggestBox that triggered the event.</param>
    /// <param name="args">Event arguments containing the selected item.</param>
    private void ApplicationAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        try
        {
            if (args.SelectedItem is ApplicationInfo selectedApp)
            {
                // ✅ CLEAN: UI only sets values, ViewModel handles validation
                sender.Text = selectedApp.Name;
                ViewModel.ApplicationAffected = selectedApp.Name;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error handling application selection: {ex.Message}");
        }
    }
}
