using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Views.Items;

internal sealed partial class ItemDetailsView
{
    public ItemDetailsView()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
