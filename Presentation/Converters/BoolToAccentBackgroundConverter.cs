using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts boolean values to accent background brush for selected state.
/// True -> AccentFillColorDefaultBrush, False -> Transparent
/// </summary>
public class BoolToAccentBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue && boolValue)
        {
            return Microsoft.UI.Xaml.Application.Current.Resources["AccentFillColorDefaultBrush"] as Brush;
        }
        
        return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
