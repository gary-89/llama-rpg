using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages.Randomizer;

internal sealed partial class RandomizerSettingsView
{
    public RandomizerSettingsView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
