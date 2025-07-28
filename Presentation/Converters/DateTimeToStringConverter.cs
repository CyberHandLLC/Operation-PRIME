using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts DateTime values to formatted string representation for display in UI.
/// Follows Microsoft IValueConverter pattern for WinUI 3 data binding.
/// </summary>
public class DateTimeToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts a DateTime value to a formatted string.
    /// </summary>
    /// <param name="value">The DateTime value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional format parameter (e.g., "short", "long").</param>
    /// <param name="language">The language for conversion (not used).</param>
    /// <returns>Formatted string representation of the DateTime.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DateTime dateTime)
            return string.Empty;

        // Use parameter to determine format, default to short date/time
        string format = parameter?.ToString()?.ToLower() ?? "short";

        return format switch
        {
            "short" => dateTime.ToString("MM/dd/yyyy HH:mm"),
            "date" => dateTime.ToString("MM/dd/yyyy"),
            "time" => dateTime.ToString("HH:mm"),
            "long" => dateTime.ToString("MMMM dd, yyyy HH:mm:ss"),
            _ => dateTime.ToString("MM/dd/yyyy HH:mm")
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
