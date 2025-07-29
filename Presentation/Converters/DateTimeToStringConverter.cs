using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts DateTime values to formatted string representation for display in UI.
/// Follows Microsoft IValueConverter pattern for WinUI 3 data binding.
/// </summary>
public class DateTimeToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts a DateTime or DateTimeOffset value to a formatted string in Eastern Time.
    /// Supports both types for backward compatibility with existing entities.
    /// All times are displayed in Eastern Time (ET) - automatically handles EDT/EST.
    /// </summary>
    /// <param name="value">The DateTime or DateTimeOffset value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional format parameter (e.g., "short", "long").</param>
    /// <param name="language">The language for conversion (not used).</param>
    /// <returns>Formatted string representation of the date/time in Eastern Time.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // Get Eastern Time Zone (handles EDT/EST automatically)
        var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        
        // Convert to Eastern Time based on input type
        DateTime easternDateTime;
        
        switch (value)
        {
            case DateTimeOffset dateTimeOffset:
                // Convert UTC DateTimeOffset to Eastern Time
                easternDateTime = TimeZoneInfo.ConvertTime(dateTimeOffset, easternTimeZone).DateTime;
                break;
            case DateTime dateTime:
                // Assume DateTime is UTC and convert to Eastern Time
                var utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, easternTimeZone);
                break;
            default:
                return string.Empty;
        }
        
        return FormatDateTime(easternDateTime, parameter, easternTimeZone);
    }
    
    /// <summary>
    /// Formats a DateTime value according to the specified parameter with timezone indicator.
    /// </summary>
    /// <param name="dateTime">The DateTime to format (already converted to Eastern Time).</param>
    /// <param name="parameter">Format parameter.</param>
    /// <param name="timeZone">The timezone for abbreviation display.</param>
    /// <returns>Formatted string with timezone indicator.</returns>
    private static string FormatDateTime(DateTime dateTime, object? parameter, TimeZoneInfo timeZone)
    {
        // Use parameter to determine format, default to short date/time
        string format = parameter?.ToString()?.ToLower() ?? "short";
        
        // Get timezone abbreviation (EDT or EST)
        string tzAbbreviation = timeZone.IsDaylightSavingTime(dateTime) ? "EDT" : "EST";

        return format switch
        {
            "short" => $"{dateTime:MM/dd/yyyy HH:mm} {"ET"}",
            "date" => dateTime.ToString("MM/dd/yyyy"),
            "time" => $"{dateTime:HH:mm} {"ET"}",
            "long" => $"{dateTime:MMMM dd, yyyy HH:mm:ss} {"ET"}",
            _ => $"{dateTime:MM/dd/yyyy HH:mm} {"ET"}"
        };
    }

    /// <summary>
    /// ConvertBack is not implemented for one-way binding.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="language">The language for conversion.</param>
    /// <returns>Not implemented.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException("ConvertBack is not supported for DateTimeToStringConverter");
    }
}
