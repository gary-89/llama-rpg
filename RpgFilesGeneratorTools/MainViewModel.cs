﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace RpgFilesGeneratorTools;

internal sealed class MainViewModel : ObservableObject
{
    private object? _selectedPage;

    public MainViewModel()
    {
        AboutCommand = new RelayCommand(ShowAboutDialog);
    }

    public ICommand AboutCommand { get; }

    public IReadOnlyList<ApplicationPage> Pages { get; } = Enum.GetValues<ApplicationPage>();

    public object? SelectedPage
    {
        get => _selectedPage;
        set => SetProperty(ref _selectedPage, value);
    }

    private static async void ShowAboutDialog()
    {
        if (App.MainRoot is null)
        {
            return;
        }

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
