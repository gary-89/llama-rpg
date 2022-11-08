using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Views.Affixes;

internal sealed partial class AffixDetailsView
{
    public AffixDetailsView()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
