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
    private static readonly SolidColorBrush DefaultBrush = Application.Current.Resources["DefaultTextForegroundThemeBrush"] as SolidColorBrush
                                                             ?? throw new InvalidOperationException("Default text block brush cannot be found");

    private static readonly SolidColorBrush EliteItemColorBrush = new(Color.FromArgb(255, 199, 179, 119));
    private static readonly SolidColorBrush RareItemColorBrush = new(Color.FromArgb(255, 255, 255, 100));
    private static readonly SolidColorBrush MagicItemColorBrush = new(Color.FromArgb(255, 105, 105, 255));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            ItemRarityType itemType => itemType switch
            {
                ItemRarityType.Magic => MagicItemColorBrush,
                ItemRarityType.Rare => RareItemColorBrush,
                ItemRarityType.Elite => EliteItemColorBrush,
                _ => DefaultBrush
            },
            UniqueMonsterType monsterType => monsterType switch
            {
                UniqueMonsterType.Boss => MagicItemColorBrush,
                UniqueMonsterType.SuperBoss => EliteItemColorBrush,
                _ => DefaultBrush
            },
            _ => DefaultBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
