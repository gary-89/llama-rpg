using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class BooleanToVisibilityConverter : IValueConverter
{
    public bool IsInverted { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool flag)
        {
            return DependencyProperty.UnsetValue;
        }

        return IsInverted
            ? (!flag ? Visibility.Visible : Visibility.Collapsed)
            : (flag ? Visibility.Visible : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
