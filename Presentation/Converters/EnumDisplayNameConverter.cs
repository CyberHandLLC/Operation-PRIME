using Microsoft.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converter that extracts the Display attribute Name from enum values.
/// Used to show user-friendly text like "5~", "10~" instead of enum names like "Five", "Ten".
/// </summary>
public class EnumDisplayNameConverter : IValueConverter
{
    /// <summary>
    /// Converts an enum value to its Display attribute Name, or the enum name if no Display attribute exists.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="language">The language (not used).</param>
    /// <returns>The Display attribute Name or the enum name as fallback.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null) return string.Empty;
        
        try
        {
            // Get the enum type and field info
            var enumType = value.GetType();
            var fieldInfo = enumType.GetField(value.ToString()!);
            
            if (fieldInfo == null) return value.ToString() ?? string.Empty;
            
            // Try to get the Display attribute
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            
            // Return the Display Name if available, otherwise return the enum name
            return displayAttribute?.Name ?? value.ToString() ?? string.Empty;
        }
        catch
        {
            // Fallback to enum name if any error occurs
            return value.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Converts back from display name to enum value (not implemented for this use case).
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="language">The language.</param>
    /// <returns>Not implemented - throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException("ConvertBack is not implemented for EnumDisplayNameConverter");
    }
}
