using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Views.Items;

internal sealed partial class ItemsView
{
    public ItemsView()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
