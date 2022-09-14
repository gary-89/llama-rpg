using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RpgFilesGeneratorTools;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        AboutCommand = new RelayCommand(ShowAboutDialog);
    }

    public ICommand AboutCommand { get; }

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