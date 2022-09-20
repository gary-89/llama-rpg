using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class ObjectEqualityToVisibilityConverter : IValueConverter
{
    public Visibility FalseValue { get; set; } = Visibility.Visible;

    public Visibility TrueValue { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return Equals(value, parameter) ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
