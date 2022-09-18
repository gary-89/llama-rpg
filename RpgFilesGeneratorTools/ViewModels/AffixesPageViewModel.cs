using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels;

internal class AffixesPageViewModel : ObservableObject
{
    private readonly RandomNumberProvider _randomNumberProvider;
    private string? _filter;

    public AffixesPageViewModel(RandomNumberProvider randomNumberProvider)
    {
        _randomNumberProvider = randomNumberProvider;
    }

    public int Number => _randomNumberProvider.GetRandomNumber();

    public string? Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }
}
