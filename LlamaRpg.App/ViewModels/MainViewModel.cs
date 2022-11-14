using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LlamaRpg.App.Models;
using LlamaRpg.App.Services;
using LlamaRpg.App.Toolkit.Async;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LlamaRpg.App.ViewModels;

internal sealed class MainViewModel : ObservableObject
{
    private readonly IUpdatesDetector _updatesDetector;

    private object? _selectedPage;
    private Version? _lastAppVersion;
    private bool _newVersionIsAvailable;

    public MainViewModel(AppConfig appConfig, IUpdatesDetector updatesDetector)
    {
        _updatesDetector = updatesDetector;

        CurrentVersion = appConfig.Version;
        AboutCommand = new RelayCommand(ShowAboutDialog);
        ExitCommand = new RelayCommand(Exit);

        TaskInitialize = new NotifyTaskCompletion<bool>(InitializeAsync());
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ICommand AboutCommand { get; }

    public ICommand ExitCommand { get; }

    public Version CurrentVersion { get; }

    public Version? LastAppVersion
    {
        get => _lastAppVersion;
        set => SetProperty(ref _lastAppVersion, value);
    }

    public bool NewVersionIsAvailable
    {
        get => _newVersionIsAvailable;
        set => SetProperty(ref _newVersionIsAvailable, value);
    }

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

    private static void Exit()
    {
        Application.Current.Exit();
    }

    private async Task<bool> InitializeAsync()
    {
        LastAppVersion = await _updatesDetector.GetLastVersionAsync(CancellationToken.None).ConfigureAwait(true);

        NewVersionIsAvailable = LastAppVersion is not null && LastAppVersion > CurrentVersion;

        return LastAppVersion is not null;
    }
}
