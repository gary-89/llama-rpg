using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Randomizer;

namespace LlamaRpg.App.ViewModels.Randomizer;

internal sealed class RandomizerSettingsViewModel : ObservableObject
{
    private int _magicItemDropRate = 5;
    private int _rareItemDropRate = 10;
    private int _eliteItemDropRate = 50;
    private int _numberOfItemsToGenerate = 1000;
    private int _monsterLevel = 50;

    public Range AffixesForMagicItems { get; } = new((int)1, (int)1);
    public Range AffixesForRareItems { get; } = new((int)3, (int)5);
    public Range AffixesForEliteItems { get; } = new((int)3, (int)7);

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
