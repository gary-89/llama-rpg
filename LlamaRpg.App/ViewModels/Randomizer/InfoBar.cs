using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LlamaRpg.App.ViewModels.Randomizer;

internal sealed class InfoBar : ObservableObject
{
    private bool _isOpen;
    private bool _isActionButtonVisible;
    private string _message = null!;
    private string _title = null!;
    private CancellationTokenSource _cancellationTokenSource = new();

    public InfoBar(ICommand command)
    {
        Command = command;
    }

    public bool IsOpen
    {
        get => _isOpen;
        private set => SetProperty(ref _isOpen, value);
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public bool IsActionButtonVisible
    {
        get => _isActionButtonVisible;
        private set => SetProperty(ref _isActionButtonVisible, value);
    }

    public ICommand Command { get; }

    public async Task ShowAsync(string title, string message, bool isActionButtonVisible)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        Title = title;
        Message = message;
        IsActionButtonVisible = isActionButtonVisible;
        IsOpen = true;

        await CloseInfoBarAsync(_cancellationTokenSource.Token).ConfigureAwait(true);
    }

    private async Task CloseInfoBarAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(15_000, cancellationToken).ConfigureAwait(true);

            cancellationToken.ThrowIfCancellationRequested();

            IsOpen = false;
        }
        catch
        {
            // ignore
        }
    }
}
