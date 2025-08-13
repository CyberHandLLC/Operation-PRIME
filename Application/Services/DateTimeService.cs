using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OperationPrime.Application.Interfaces;
using OperationPrime.Domain.Configuration;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for date/time business logic operations.
/// Handles date/time combinations and validations following Clean Architecture principles.
/// </summary>
public class DateTimeService : IDateTimeService
{
    private readonly ILogger<DateTimeService> _logger;
    private readonly ApplicationSettings _settings;

    public DateTimeService(ILogger<DateTimeService> logger, IOptions<ApplicationSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Combines a date and time into a single DateTimeOffset.
    /// </summary>
    /// <param name="date">The date component</param>
    /// <param name="time">The time component</param>
    /// <returns>Combined DateTimeOffset maintaining the original offset</returns>
    public DateTimeOffset CombineDateAndTime(DateTimeOffset date, TimeSpan time)
    {
        return new DateTimeOffset(
            date.Year, date.Month, date.Day,
            time.Hours, time.Minutes, time.Seconds,
            date.Offset);
    }

    /// <summary>
    /// Validates that the issue start time is not in the future.
    /// Normalizes the input to the configured Eastern time zone to account for UI offset issues,
    /// then compares against the current Eastern time with a small tolerance.
    /// </summary>
    /// <param name="timeIssueStarted">The time when the issue started</param>
    /// <returns>True if the time is valid (not in future)</returns>
    public bool ValidateIssueStartTime(DateTimeOffset? timeIssueStarted)
    {
        if (!timeIssueStarted.HasValue) return false;
        
        // IMPORTANT: UI controls return correct Eastern Time but with wrong timezone offset
        // Treat the DateTime component as Eastern Time and create proper Eastern DateTimeOffset
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.DefaultTimeZone);
        var easternOffset = easternTimeZone.GetUtcOffset(timeIssueStarted.Value.DateTime);
        var inputEasternOffset = new DateTimeOffset(timeIssueStarted.Value.DateTime, easternOffset);
        
        var currentEastern = GetCurrentEasternTime();
        var maxAllowedEastern = currentEastern.AddMinutes(5);
        
        var isValid = inputEasternOffset <= maxAllowedEastern;
        
        // Debug logging
        _logger.LogDebug("Eastern Time Validation: Input={OriginalTime}, Corrected={CorrectedTime}, Current={CurrentTime}, MaxAllowed={MaxTime}, IsValid={IsValid}", 
            timeIssueStarted.Value, inputEasternOffset, currentEastern, maxAllowedEastern, isValid);
        
        return isValid;
    }

    /// <summary>
    /// Validates that the reported time is not before the issue start time.
    /// Normalizes both inputs to the configured Eastern time zone to account for UI offset issues
    /// and compares them in that zone.
    /// </summary>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <returns>True if the timing is logical</returns>
    public bool ValidateReportedTime(DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported)
    {
        if (!timeIssueStarted.HasValue || !timeReported.HasValue) return false;
        
        // IMPORTANT: UI controls return correct Eastern Time but with wrong timezone offset
        // Treat the DateTime components as Eastern Time and create proper Eastern DateTimeOffsets
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.DefaultTimeZone);
        
        var issueStartOffset = easternTimeZone.GetUtcOffset(timeIssueStarted.Value.DateTime);
        var issueStartEastern = new DateTimeOffset(timeIssueStarted.Value.DateTime, issueStartOffset);
        
        var reportedOffset = easternTimeZone.GetUtcOffset(timeReported.Value.DateTime);
        var reportedEastern = new DateTimeOffset(timeReported.Value.DateTime, reportedOffset);
        
        // Reported time should be >= issue start time (in Eastern)
        return reportedEastern >= issueStartEastern;
    }

    /// <summary>
    /// Gets the current UTC time for default values.
    /// </summary>
    /// <returns>Current DateTimeOffset in UTC</returns>
    public DateTimeOffset GetCurrentUtcTime()
    {
        return DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the current time in Eastern Time zone for UI initialization.
    /// This ensures UI controls show the correct Eastern Time by default.
    /// </summary>
    /// <returns>Current DateTimeOffset in Eastern Time</returns>
    public DateTimeOffset GetCurrentEasternTime()
    {
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.DefaultTimeZone);
        var utcNow = DateTimeOffset.UtcNow;
        
        // Convert UTC to Eastern Time, ensuring proper Eastern offset
        var easternTime = TimeZoneInfo.ConvertTime(utcNow, easternTimeZone);
        
        // Debug: Show what we're generating
        _logger.LogDebug("GetCurrentEasternTime: UTC={UtcInput}, Eastern={EasternOutput}, Offset={EasternOffset}", 
            utcNow, easternTime, easternTime.Offset);
        
        return easternTime;
    }

    /// <summary>
    /// Gets the current local time for UI initialization.
    /// This ensures validation works correctly with user's actual local time.
    /// </summary>
    /// <returns>Current DateTimeOffset in local timezone</returns>
    public DateTimeOffset GetCurrentLocalTime()
    {
        return DateTimeOffset.Now;
    }
} 