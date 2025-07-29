using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts boolean values to accent foreground brush for selected state.
/// True -> AccentTextFillColorPrimaryBrush, False -> TextFillColorPrimaryBrush
/// </summary>
public class BoolToAccentForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue && boolValue)
        {
            return Microsoft.UI.Xaml.Application.Current.Resources["AccentTextFillColorPrimaryBrush"] as Brush;
        }
        
        return Microsoft.UI.Xaml.Application.Current.Resources["TextFillColorPrimaryBrush"] as Brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
