using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Views.Affixes;

internal sealed partial class AffixesView
{
    public AffixesView()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
