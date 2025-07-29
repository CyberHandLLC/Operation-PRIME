using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts boolean values to Visibility values.
/// True -> Visible, False -> Collapsed
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        
        return false;
    }
}
