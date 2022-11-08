using LlamaRpg.App.ViewModels;

namespace LlamaRpg.App.Pages;

internal sealed partial class RandomizerPage
{
    public RandomizerPage()
    {
        InitializeComponent();
    }

    public RandomizerPageViewModel ViewModel => (RandomizerPageViewModel)DataContext;
}
