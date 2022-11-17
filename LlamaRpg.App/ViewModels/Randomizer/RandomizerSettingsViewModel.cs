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

    public Range AffixesForMagicItems { get; } = new(1, 1);
    public Range AffixesForRareItems { get; } = new(3, 3);
    public Range AffixesForEliteItems { get; } = new(3, 5);

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
