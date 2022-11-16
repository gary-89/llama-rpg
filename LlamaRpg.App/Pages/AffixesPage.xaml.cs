using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages;

internal sealed partial class AffixesPage
{
    public AffixesPage()
    {
        InitializeComponent();
    }

    public AffixesPageViewModel ViewModel => (AffixesPageViewModel)DataContext;
}
