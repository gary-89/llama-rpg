using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using RpgFilesGeneratorTools.Models;
using Windows.UI;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class RarityTypeToForegroundBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush s_defaultBrush = Application.Current.Resources["DefaultTextForegroundThemeBrush"] as SolidColorBrush
                                                             ?? throw new InvalidOperationException("Default text block brush cannot be found");

    private static readonly SolidColorBrush s_eliteItemColorBrush = new(Color.FromArgb(255, 199, 179, 119));
    private static readonly SolidColorBrush s_rareItemColorBrush = new(Color.FromArgb(255, 255, 255, 100));
    private static readonly SolidColorBrush s_magicItemColorBrush = new(Color.FromArgb(255, 105, 105, 255));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not ItemRarityType type)
        {
            return DependencyProperty.UnsetValue;
        }

        return type switch
        {
            ItemRarityType.Magic => s_magicItemColorBrush,
            ItemRarityType.Rare => s_rareItemColorBrush,
            ItemRarityType.Elite => s_eliteItemColorBrush,
            _ => s_defaultBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
