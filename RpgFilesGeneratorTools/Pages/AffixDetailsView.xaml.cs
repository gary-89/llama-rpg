using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages;

internal sealed partial class AffixDetailsView
{
    public AffixDetailsView()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
