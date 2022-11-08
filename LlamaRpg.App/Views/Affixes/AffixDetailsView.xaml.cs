using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Views.Affixes;

internal sealed partial class AffixDetailsView
{
    public AffixDetailsView()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
