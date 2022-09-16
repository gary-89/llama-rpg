using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RpgFilesGeneratorTools
{
    internal sealed class MainViewModel : ObservableObject
    {
        private readonly TestService _testService;

        private ApplicationPage _selectedPage = ApplicationPage.Home;

        public MainViewModel(TestService testService)
        {
            _testService = testService;

            AboutCommand = new RelayCommand(ShowAboutDialog);
        }

        public int RandomNumber => _testService.Test();

        public ICommand AboutCommand { get; }

        public IReadOnlyList<ApplicationPage> Pages { get; } = Enum.GetValues<ApplicationPage>();

        public ApplicationPage SelectedPage
        {
            get => _selectedPage;
            set => SetProperty(ref _selectedPage, value);
        }

        private static async void ShowAboutDialog()
        {
            var dialog = new ContentDialog
            {
                XamlRoot = App.MainRoot.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "About",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                Content = new AboutView()
            };

            await dialog.ShowAsync();
        }
    }
}
