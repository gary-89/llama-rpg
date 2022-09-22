using Microsoft.UI.Xaml.Controls;
using RpgFilesGeneratorTools.Pages;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class MenuItemToPageConverter
{
    public static Page? Convert(object? value)
    {
        if (value is not ApplicationPage page)
        {
            return default;
        }

        return page switch
        {
            ApplicationPage.Items => new ItemsPage(),
            ApplicationPage.Affixes => new AffixesPage(),
            ApplicationPage.Monsters => new MonstersPage(),
            ApplicationPage.Maps => new MapsPage(),
            ApplicationPage.Randomizer => new RandomizerPage(),
            _ => new ItemsPage()
        };
    }

    public static ApplicationPage ConvertBack(object value) => (ApplicationPage)value;
}
