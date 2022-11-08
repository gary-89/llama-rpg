using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Views.Items;

internal sealed partial class ItemDetailsView
{
    public ItemDetailsView()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
