using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Views.Affixes;

internal sealed partial class AffixesView
{
    public AffixesView()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
