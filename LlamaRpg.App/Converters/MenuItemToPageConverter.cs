using LlamaRpg.App.Models;
using LlamaRpg.App.Pages;
using Microsoft.UI.Xaml.Controls;

namespace LlamaRpg.App.Converters;

internal sealed class MenuItemToPageConverter
{
    public static Page Convert(object? value)
    {
        if (value is not ApplicationPage page)
        {
            return new ItemsPage();
        }

        return page switch
        {
            ApplicationPage.Items => new ItemsPage(),
            ApplicationPage.Affixes => new AffixesPage(),
            ApplicationPage.Randomizer => new RandomizerPage(),
            _ => new ItemsPage()
        };
    }

    public static ApplicationPage ConvertBack(object value) => (ApplicationPage)value;
}
