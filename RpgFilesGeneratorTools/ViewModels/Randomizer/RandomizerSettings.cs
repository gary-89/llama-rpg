using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class Range
{
    public Range(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int Min { get; set; }
    public int Max { get; set; }
}

internal sealed class RandomizerSettings : ObservableObject
{
    private int _magicItemDropRate = 5;
    private int _rareItemDropRate = 10;
    private int _eliteItemDropRate = 50;
    private int _numberOfItemsToGenerate = 1000;
    private int _monsterLevel = 50;

    public Range AffixesForMagicItems { get; } = new(1, 1);
    public Range AffixesForRareItems { get; } = new(3, 5);
    public Range AffixesForEliteItems { get; } = new(3, 7);

    public int MonsterLevel
    {
        get => _monsterLevel;
        set => SetProperty(ref _monsterLevel, value);
    }

    public int MagicItemDropRate
    {
        get => _magicItemDropRate;
        set => SetProperty(ref _magicItemDropRate, value);
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
