using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace OperationPrime.Presentation.Converters;

/// <summary>
/// Converts boolean values to accent border brush for selected state.
/// True -> AccentFillColorDefaultBrush, False -> ControlStrokeColorDefaultBrush
/// </summary>
public class BoolToAccentBorderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue && boolValue)
        {
            return Microsoft.UI.Xaml.Application.Current.Resources["AccentFillColorDefaultBrush"] as Brush;
        }
        
        return Microsoft.UI.Xaml.Application.Current.Resources["ControlStrokeColorDefaultBrush"] as Brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
