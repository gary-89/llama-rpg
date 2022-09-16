using Microsoft.UI.Xaml.Controls;
using RpgFilesGeneratorTools.Pages;
using System;

namespace RpgFilesGeneratorTools.Converters
{
    internal sealed class MenuItemToPageConverter
    {
        public static Page Convert(object? value)
        {
            if (value is null)
            {
                return new HomePage();
            }

            if (value is not ApplicationPage page)
            {
                throw new InvalidCastException();
            }

            switch (page)
            {
                case ApplicationPage.Home:
                default:
                    return new HomePage();

                case ApplicationPage.Apps:
                    return new AppPage();
            }
        }

        public static ApplicationPage ConvertBack(object value) => (ApplicationPage)value;
    }
}
