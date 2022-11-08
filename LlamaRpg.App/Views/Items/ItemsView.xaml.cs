using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Views.Items;

internal sealed partial class ItemsView
{
    public ItemsView()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
