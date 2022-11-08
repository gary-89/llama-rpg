using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages.Randomizer;

internal sealed partial class ItemsGeneratorView
{
    public ItemsGeneratorView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
