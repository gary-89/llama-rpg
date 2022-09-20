using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages;

internal sealed partial class ItemDetails
{
    public ItemDetails()
    {
        InitializeComponent();
    }

    public ItemsPageViewModel ViewModel => (ItemsPageViewModel)DataContext;
}
