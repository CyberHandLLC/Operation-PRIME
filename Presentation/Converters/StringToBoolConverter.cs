using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts string values to boolean for conditional UI states.
/// Returns true if string is not null/empty, false otherwise.
/// Follows Microsoft IValueConverter pattern for WinUI 3 data binding.
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts a string value to boolean.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="language">The language for conversion (not used).</param>
    /// <returns>True if string has value, false if null/empty.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }

        return false;
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
        throw new NotImplementedException("ConvertBack is not supported for StringToBoolConverter");
    }
}
