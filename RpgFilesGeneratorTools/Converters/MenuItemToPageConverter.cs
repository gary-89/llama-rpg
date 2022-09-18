using Microsoft.UI.Xaml.Controls;
using RpgFilesGeneratorTools.Pages;
using System;

namespace RpgFilesGeneratorTools.Converters;

internal sealed class MenuItemToPageConverter
{
    public static Page Convert(object? value)
    {
        if (value is null)
        {
            return new AffixesPage();
        }

        if (value is not ApplicationPage page)
        {
            throw new InvalidCastException();
        }

        switch (page)
        {
            case ApplicationPage.Affixes:
            default:
                return new AffixesPage();

            case ApplicationPage.Monsters:
                return new MonstersPage();

            case ApplicationPage.Items:
                return new ItemsPage();

            case ApplicationPage.Maps:
                return new MapsPage();
        }
    }

    public static ApplicationPage ConvertBack(object value) => (ApplicationPage)value;
}