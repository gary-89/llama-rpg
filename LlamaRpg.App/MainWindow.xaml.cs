using System;
using LlamaRpg.App.ViewModels;
using Microsoft.UI.Xaml;

namespace LlamaRpg.App;

internal sealed partial class MainWindow
{
    public MainWindow(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;

        InitializeComponent();

        SetTitleBar();
    }

    public MainViewModel MainViewModel { get; }

    private void SetTitleBar()
    {
        var isWindows11 = Environment.OSVersion.Version.Build >= 22000;

        if (isWindows11)
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }
}
