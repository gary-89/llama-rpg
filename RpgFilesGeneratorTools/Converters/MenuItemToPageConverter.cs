using Microsoft.UI.Xaml.Controls;
using RpgFilesGeneratorTools.Pages;

namespace RpgFilesGeneratorTools.Converters
{
    internal sealed class MenuItemToPageConverter
    {
        //public object Convert(object value, Type targetType, object parameter, string language)
        //{

        //}

        //public object ConvertBack(object value, Type targetType, object parameter, string language)
        //{
        //    throw new NotImplementedException();
        //}

        public static Page Convert(ApplicationPage page)
        {
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
