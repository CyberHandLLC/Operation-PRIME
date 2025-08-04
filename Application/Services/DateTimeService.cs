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
    /// Converts both times to UTC for proper comparison regardless of timezone.
    /// </summary>
    /// <param name="timeIssueStarted">The time when the issue started</param>
    /// <returns>True if the time is valid (not in future)</returns>
    public bool ValidateIssueStartTime(DateTimeOffset? timeIssueStarted)
    {
        if (!timeIssueStarted.HasValue) return false;
        
        // IMPORTANT: UI controls return correct Eastern Time but with wrong timezone offset
        // Treat the DateTime component as Eastern Time and create proper Eastern DateTimeOffset
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var easternOffset = easternTimeZone.GetUtcOffset(timeIssueStarted.Value.DateTime);
        var inputEasternOffset = new DateTimeOffset(timeIssueStarted.Value.DateTime, easternOffset);
        
        var currentEastern = GetCurrentEasternTime();
        var maxAllowedEastern = currentEastern.AddMinutes(5);
        
        var isValid = inputEasternOffset <= maxAllowedEastern;
        
        // Debug logging
        Console.WriteLine($"[DateTimeService] Eastern Time Validation:");
        Console.WriteLine($"  Input Time (original): {timeIssueStarted.Value}");
        Console.WriteLine($"  Input Time (corrected Eastern): {inputEasternOffset}");
        Console.WriteLine($"  Current Eastern: {currentEastern}");
        Console.WriteLine($"  Max Allowed: {maxAllowedEastern}");
        Console.WriteLine($"  Is Valid: {isValid}");
        
        return isValid;
    }

    /// <summary>
    /// Validates that the reported time is not before the issue start time.
    /// Converts both times to UTC for proper comparison regardless of timezone.
    /// </summary>
    /// <param name="timeIssueStarted">When the issue started</param>
    /// <param name="timeReported">When the issue was reported</param>
    /// <returns>True if the timing is logical</returns>
    public bool ValidateReportedTime(DateTimeOffset? timeIssueStarted, DateTimeOffset? timeReported)
    {
        if (!timeIssueStarted.HasValue || !timeReported.HasValue) return false;
        
        // IMPORTANT: UI controls return correct Eastern Time but with wrong timezone offset
        // Treat the DateTime components as Eastern Time and create proper Eastern DateTimeOffsets
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        
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
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var utcNow = DateTimeOffset.UtcNow;
        
        // Convert UTC to Eastern Time, ensuring proper Eastern offset
        var easternTime = TimeZoneInfo.ConvertTime(utcNow, easternTimeZone);
        
        // Debug: Show what we're generating
        Console.WriteLine($"[DateTimeService] GetCurrentEasternTime:");
        Console.WriteLine($"  UTC Input: {utcNow}");
        Console.WriteLine($"  Eastern Output: {easternTime}");
        Console.WriteLine($"  Eastern Offset: {easternTime.Offset}");
        
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