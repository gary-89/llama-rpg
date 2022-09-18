using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages;

internal sealed partial class AffixesPage
{
    public AffixesPage()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}