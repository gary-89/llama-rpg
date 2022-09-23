using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class InfoBar : ObservableObject
{
    private bool _isOpen;
    private string _message = null!;

    public InfoBar(ICommand actionCommand)
    {
        OpenLastGeneratedFileCommand = actionCommand;
    }

    public bool IsOpen
    {
        get => _isOpen;
        set => SetProperty(ref _isOpen, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public ICommand OpenLastGeneratedFileCommand { get; }
}
