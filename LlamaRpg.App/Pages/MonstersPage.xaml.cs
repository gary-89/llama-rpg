using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages;

internal sealed partial class MonstersPage
{
    public MonstersPage()
    {
        InitializeComponent();
    }

    public MonstersPageViewModel ViewModel => (MonstersPageViewModel)DataContext;
}
