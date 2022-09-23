using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class RandomizerSettings : ObservableObject
{
    private int _rareItemDropRate = 10;
    private int _uniqueItemDropRate = 50;
    private int _numberOfItemsToGenerate = 100;

    public int RareItemDropRate
    {
        get => _rareItemDropRate;
        set => SetProperty(ref _rareItemDropRate, value);
    }

    public int UniqueItemDropRate
    {
        get => _uniqueItemDropRate;
        set => SetProperty(ref _uniqueItemDropRate, value);
    }

    public int NumberOfItemsToGenerate
    {
        get => _numberOfItemsToGenerate;
        set => SetProperty(ref _numberOfItemsToGenerate, value);
    }
}
