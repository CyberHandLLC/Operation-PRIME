using OperationPrime.Application.Interfaces;

namespace OperationPrime.Application.Services;

/// <summary>
/// Service implementation for date/time business logic operations.
/// Handles date/time combinations and validations following Clean Architecture principles.
/// </summary>
public class DateTimeService : IDateTimeService
{
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
    /// </summary>
    /// <param name="timeIssueStarted">The time when the issue started</param>
    /// <returns>True if the time is valid (not in future)</returns>
    public bool ValidateIssueStartTime(DateTimeOffset? timeIssueStarted)
    {
        if (!timeIssueStarted.HasValue)
            return false;

        // Allow some tolerance for clock differences (5 minutes)
        var maxAllowedTime = DateTimeOffset.UtcNow.AddMinutes(5);
        return timeIssueStarted.Value <= maxAllowedTime;
    }

    /// <summary>
    /// Validates that the reported time is not before the issue start time.
    /// </summary>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <returns>True if the timing is logical</returns>
    public bool ValidateReportedTime(DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported)
    {
        if (!timeIssueStarted.HasValue || !timeReported.HasValue)
            return false;

        // Reported time should be equal to or after issue start time
        return timeReported.Value >= timeIssueStarted.Value;
    }

    /// <summary>
    /// Gets the current UTC time for default values.
    /// </summary>
    /// <returns>Current DateTimeOffset in UTC</returns>
    public DateTimeOffset GetCurrentUtcTime()
    {
        return DateTimeOffset.UtcNow;
    }
} 