using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts boolean values to Visibility values with inverse logic.
/// True -> Collapsed, False -> Visible
/// </summary>
public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Collapsed;
        }
        
        return true;
    }
}
