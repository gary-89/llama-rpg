using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages;

internal sealed partial class ItemsPage
{
    public ItemsPage()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
