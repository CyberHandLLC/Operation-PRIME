namespace OperationPrime.Application.Interfaces;

/// <summary>
/// Service interface for date/time business logic operations.
/// Handles date/time combinations and validations following Clean Architecture principles.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Combines a date and time into a single DateTimeOffset.
    /// </summary>
    /// <param name="date">The date component</param>
    /// <param name="time">The time component</param>
    /// <returns>Combined DateTimeOffset maintaining the original offset</returns>
    DateTimeOffset CombineDateAndTime(DateTimeOffset date, TimeSpan time);

    /// <summary>
    /// Validates that the issue start time is not in the future.
    /// </summary>
    /// <param name="timeIssueStarted">The time when the issue started</param>
    /// <returns>True if the time is valid (not in future)</returns>
    bool ValidateIssueStartTime(DateTimeOffset? timeIssueStarted);

    /// <summary>
    /// Validates that the reported time is not before the issue start time.
    /// </summary>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <returns>True if the timing is logical</returns>
    bool ValidateReportedTime(DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported);

    /// <summary>
    /// Gets the current UTC time for default values.
    /// </summary>
    /// <returns>Current DateTimeOffset in UTC</returns>
    DateTimeOffset GetCurrentUtcTime();

    /// <summary>
    /// Gets the current time in Eastern Time zone for UI initialization.
    /// This ensures UI controls show the correct Eastern Time by default.
    /// </summary>
    /// <returns>Current DateTimeOffset in Eastern Time</returns>
    DateTimeOffset GetCurrentEasternTime();

    /// <summary>
    /// Gets the current local time for UI initialization.
    /// This ensures validation works correctly with user's actual local time.
    /// </summary>
    /// <returns>Current DateTimeOffset in local timezone</returns>
    DateTimeOffset GetCurrentLocalTime();
} 