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
    private bool _displayInfoBar;
    private string? _infoBarMessage;
    private bool _isDownloadingNewVersion;
    private DownloadUpdatesState _downloadUpdatesState = DownloadUpdatesState.None;

    public MainViewModel(AppConfig appConfig, IUpdatesDetector updatesDetector)
    {
        _updatesDetector = updatesDetector;

        CurrentVersion = appConfig.CurrentVersion;
        AboutCommand = new RelayCommand(ShowAboutDialog);
        DownloadLastVersionCommand = new AsyncRelayCommand(DownloadLastVersionAsync);
        ExitCommand = new RelayCommand(Exit);

        TaskInitialize = new NotifyTaskCompletion<bool>(InitializeAsync());
    }

    public NotifyTaskCompletion<bool> TaskInitialize { get; }

    public ICommand AboutCommand { get; }

    public ICommand DownloadLastVersionCommand { get; }

    public ICommand ExitCommand { get; }

    public Version CurrentVersion { get; }

    public bool IsDownloadingNewVersion
    {
        get => _isDownloadingNewVersion;
        private set => SetProperty(ref _isDownloadingNewVersion, value);
    }

    public DownloadUpdatesState DownloadUpdatesState
    {
        get => _downloadUpdatesState;
        private set => SetProperty(ref _downloadUpdatesState, value);
    }

    public bool DisplayInfoBar
    {
        get => _displayInfoBar;
        set => SetProperty(ref _displayInfoBar, value);
    }

    public string? InfoBarMessage
    {
        get => _infoBarMessage;
        private set => SetProperty(ref _infoBarMessage, value);
    }

    public Version? LastAppVersion
    {
        get => _lastAppVersion;
        private set => SetProperty(ref _lastAppVersion, value);
    }

    public bool NewVersionIsAvailable
    {
        get => _newVersionIsAvailable;
        private set => SetProperty(ref _newVersionIsAvailable, value);
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

    private async Task DownloadLastVersionAsync(CancellationToken cancellationToken)
    {
        DisplayInfoBar = true;

        IsDownloadingNewVersion = true;

        DownloadUpdatesState = DownloadUpdatesState.Downloading;

        InfoBarMessage = "Downloading the last available version... Please, wait...";

        await Task.Delay(2_000, cancellationToken).ConfigureAwait(true);

        var destinationPath = await _updatesDetector.DownloadLastVersionAsync(cancellationToken).ConfigureAwait(true);

        IsDownloadingNewVersion = false;

        DownloadUpdatesState = destinationPath is null ? DownloadUpdatesState.Failed : DownloadUpdatesState.Succeeded;

        InfoBarMessage = destinationPath is null
            ? "Something went wrong. The new version cannot be downloaded."
            : $"A new version has been downloaded on your Desktop folder: {destinationPath}.\nPlease close the current app, and run the downloaded one.";
    }
}
