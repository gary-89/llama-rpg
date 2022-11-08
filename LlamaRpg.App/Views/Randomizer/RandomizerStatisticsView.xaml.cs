using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages.Randomizer;

internal sealed partial class RandomizerStatisticsView
{
    public RandomizerStatisticsView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
