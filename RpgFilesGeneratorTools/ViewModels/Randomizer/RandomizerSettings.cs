using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class RandomizerSettings : ObservableObject
{
    private int _rareItemDropRate = 10;
    private int _eliteItemDropRate = 50;
    private int _numberOfItemsToGenerate = 1000;
    private int _characterLevel = 1;

    public int CharacterLevel
    {
        get => _characterLevel;
        set => SetProperty(ref _characterLevel, value);
    }

    public int RareItemDropRate
    {
        get => _rareItemDropRate;
        set => SetProperty(ref _rareItemDropRate, value);
    }

    public int EliteItemDropRate
    {
        get => _eliteItemDropRate;
        set => SetProperty(ref _eliteItemDropRate, value);
    }

    public int NumberOfItemsToGenerate
    {
        get => _numberOfItemsToGenerate;
        set => SetProperty(ref _numberOfItemsToGenerate, value);
    }

    public ObservableCollection<ItemTypeWeightDrop> ItemTypeWeights { get; } = new();
}
