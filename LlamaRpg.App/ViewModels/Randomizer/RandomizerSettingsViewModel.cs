using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LlamaRpg.Models.Items;
using Range = LlamaRpg.Models.Range;

namespace LlamaRpg.App.ViewModels.Randomizer;

internal sealed class RandomizerSettingsViewModel : ObservableObject
{
    private int _magicItemDropRate = 5;
    private int _rareItemDropRate = 10;
    private int _eliteItemDropRate = 50;
    private int _numberOfItemsToGenerate = 1000;
    private int _monsterLevel = 50;
    private int _minTotalAffixesForMagicItems = 0;
    private int _maxTotalAffixesForMagicItems = 2;
    private int _minTotalAffixesForRareItems = 2;
    private int _maxTotalAffixesForRareItems = 4;

    public Range PrefixesForMagicItems { get; } = new(0, 1);
    public Range SuffixesForMagicItems { get; } = new(0, 1);

    public Range PrefixesForRareItems { get; } = new(1, 2);
    public Range SuffixesForRareItems { get; } = new(1, 2);

    public Range TotalAffixesForEliteItems { get; } = new(3, 4);

    public RandomizerSettingsViewModel()
    {
        PrefixesForMagicItems.RangeChanged += AffixesForMagicItemsOnRangeChanged;
        SuffixesForMagicItems.RangeChanged += AffixesForMagicItemsOnRangeChanged;

        PrefixesForRareItems.RangeChanged += AffixesForRareItemsOnRangeChanged;
        SuffixesForRareItems.RangeChanged += AffixesForRareItemsOnRangeChanged;
    }

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

    public int MinTotalAffixesForMagicItems
    {
        get => _minTotalAffixesForMagicItems;
        set => SetProperty(ref _minTotalAffixesForMagicItems, value);
    }

    public int MaxTotalAffixesForMagicItems
    {
        get => _maxTotalAffixesForMagicItems;
        set => SetProperty(ref _maxTotalAffixesForMagicItems, value);
    }

    public int MinTotalAffixesForRareItems
    {
        get => _minTotalAffixesForRareItems;
        set => SetProperty(ref _minTotalAffixesForRareItems, value);
    }

    public int MaxTotalAffixesForRareItems
    {
        get => _maxTotalAffixesForRareItems;
        set => SetProperty(ref _maxTotalAffixesForRareItems, value);
    }

    public ObservableCollection<ItemTypeWeightDrop> ItemTypeWeights { get; } = new();

    private void AffixesForMagicItemsOnRangeChanged(object? sender, EventArgs e)
    {
        MinTotalAffixesForMagicItems = PrefixesForMagicItems.Min + SuffixesForMagicItems.Min;
        MaxTotalAffixesForMagicItems = PrefixesForMagicItems.Max + SuffixesForMagicItems.Max;
    }

    private void AffixesForRareItemsOnRangeChanged(object? sender, EventArgs e)
    {
        MinTotalAffixesForRareItems = PrefixesForRareItems.Min + SuffixesForRareItems.Min;
        MaxTotalAffixesForRareItems = PrefixesForRareItems.Max + SuffixesForRareItems.Max;
    }
}
