using System;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Monsters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace LlamaRpg.App.Converters;

internal sealed class EnumToForegroundBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush s_defaultBrush = Application.Current.Resources["DefaultTextForegroundThemeBrush"] as SolidColorBrush
                                                             ?? throw new InvalidOperationException("Default text block brush cannot be found");

    private static readonly SolidColorBrush s_eliteItemColorBrush = new(Color.FromArgb(255, 199, 179, 119));
    private static readonly SolidColorBrush s_rareItemColorBrush = new(Color.FromArgb(255, 255, 255, 100));
    private static readonly SolidColorBrush s_magicItemColorBrush = new(Color.FromArgb(255, 105, 105, 255));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            ItemRarityType itemType => itemType switch
            {
                ItemRarityType.Magic => s_magicItemColorBrush,
                ItemRarityType.Rare => s_rareItemColorBrush,
                ItemRarityType.Elite => s_eliteItemColorBrush,
                _ => s_defaultBrush
            },
            UniqueMonsterType monsterType => monsterType switch
            {
                UniqueMonsterType.Boss => s_magicItemColorBrush,
                UniqueMonsterType.SuperBoss => s_eliteItemColorBrush,
                _ => s_defaultBrush
            },
            _ => s_defaultBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
