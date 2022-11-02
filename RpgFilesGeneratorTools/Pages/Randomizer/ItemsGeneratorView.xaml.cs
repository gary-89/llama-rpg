using RpgFilesGeneratorTools.ViewModels;

namespace RpgFilesGeneratorTools.Pages.Randomizer;

internal sealed partial class ItemsGeneratorView
{
    public ItemsGeneratorView()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
