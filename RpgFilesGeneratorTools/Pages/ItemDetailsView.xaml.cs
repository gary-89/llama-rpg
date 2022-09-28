using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages;

internal sealed partial class ItemDetailsView
{
    public ItemDetailsView()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
