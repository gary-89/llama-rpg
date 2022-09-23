using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class DoubleToStringFormatConverter : IValueConverter
{
    public object Convert(object? value, Type? targetType, object? parameter, string? language)
    {
        return value is not double number
            ? DependencyProperty.UnsetValue
            : Convert(number, parameter is null ? string.Empty : (string)parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    private static string Convert(double value, string format)
    {
        return value.ToString(format);
    }
}
