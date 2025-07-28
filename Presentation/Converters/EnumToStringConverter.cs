using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts enum values to their string representation for display in UI.
/// Follows Microsoft IValueConverter pattern for WinUI 3 data binding.
/// </summary>
public class EnumToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts an enum value to its string representation.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="language">The language for conversion (not used).</param>
    /// <returns>String representation of the enum value.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
            return string.Empty;

        // Convert enum to string representation, ensuring non-null return
        return value.ToString() ?? string.Empty;
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
        throw new NotImplementedException("ConvertBack is not supported for EnumToStringConverter");
    }
}
