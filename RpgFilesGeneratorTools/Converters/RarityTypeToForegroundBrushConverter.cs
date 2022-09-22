﻿using System;
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

    private static readonly SolidColorBrush s_uniqueItemColorBrush = new(Color.FromArgb(255, 255, 215, 0));
    private static readonly SolidColorBrush s_rareItemColorBrush = new(Color.FromArgb(255, 20, 93, 160));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not ItemRarityType type)
        {
            return DependencyProperty.UnsetValue;
        }

        return type switch
        {
            ItemRarityType.Unique => s_uniqueItemColorBrush,
            ItemRarityType.Rare => s_rareItemColorBrush,
            _ => s_defaultBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
