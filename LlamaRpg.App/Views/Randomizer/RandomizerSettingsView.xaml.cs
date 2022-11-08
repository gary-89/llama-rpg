using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages.Randomizer;

internal sealed partial class RandomizerSettingsView
{
    public RandomizerSettingsView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
