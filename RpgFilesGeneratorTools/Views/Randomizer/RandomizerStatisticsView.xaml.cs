using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages.Randomizer;

internal sealed partial class RandomizerStatisticsView
{
    public RandomizerStatisticsView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
