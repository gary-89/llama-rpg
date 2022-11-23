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

    private int _minTotalAffixesForMagicItems = 1;
    private int _maxTotalAffixesForMagicItems = 2;
    private int _minTotalAffixesForRareItems = 3;
    private int _maxTotalAffixesForRareItems = 4;

    private int _mandatoryAffixesForMagicItems = 1;
    private int _mandatoryAffixesForRareItems = 3;

    public RandomizerSettingsViewModel()
    {
        PrefixesForMagicItems.RangeChanged += AffixesForMagicItemsOnRangeChanged;
        SuffixesForMagicItems.RangeChanged += AffixesForMagicItemsOnRangeChanged;

        PrefixesForRareItems.RangeChanged += AffixesForRareItemsOnRangeChanged;
        SuffixesForRareItems.RangeChanged += AffixesForRareItemsOnRangeChanged;
    }

    public Range PrefixesForMagicItems { get; } = new(0, 1);
    public Range SuffixesForMagicItems { get; } = new(0, 1);

    public Range PrefixesForRareItems { get; } = new(1, 2);
    public Range SuffixesForRareItems { get; } = new(1, 2);

    public Range TotalAffixesForEliteItems { get; } = new(3, 4);

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

    public int MandatoryAffixesForMagicItems
    {
        get => _mandatoryAffixesForMagicItems;
        set
        {
            if (SetProperty(ref _mandatoryAffixesForMagicItems, value))
            {
                RefreshAffixesSettings(ItemRarityType.Magic);
            }
        }
    }

    public int MandatoryAffixesForRareItems
    {
        get => _mandatoryAffixesForRareItems;
        set
        {
            if (SetProperty(ref _mandatoryAffixesForRareItems, value))
            {
                RefreshAffixesSettings(ItemRarityType.Rare);
            }
        }
    }

    public ObservableCollection<ItemTypeWeightDrop> ItemTypeWeights { get; } = new();

    private void AffixesForMagicItemsOnRangeChanged(object? sender, EventArgs e)
    {
        RefreshAffixesSettings(ItemRarityType.Magic);
    }

    private void AffixesForRareItemsOnRangeChanged(object? sender, EventArgs e)
    {
        RefreshAffixesSettings(ItemRarityType.Rare);
    }

    private void RefreshAffixesSettings(ItemRarityType rarityType)
    {
        switch (rarityType)
        {
            case ItemRarityType.Magic:
                MinTotalAffixesForMagicItems = Math.Max(PrefixesForMagicItems.Min + SuffixesForMagicItems.Min, MandatoryAffixesForMagicItems);
                MaxTotalAffixesForMagicItems = PrefixesForMagicItems.Max + SuffixesForMagicItems.Max;
                break;

            case ItemRarityType.Rare:
                MinTotalAffixesForRareItems = Math.Max(PrefixesForRareItems.Min + SuffixesForRareItems.Min, MandatoryAffixesForRareItems);
                MaxTotalAffixesForRareItems = PrefixesForRareItems.Max + SuffixesForRareItems.Max;
                break;

            case ItemRarityType.Normal:
            case ItemRarityType.Elite:
            default:
                throw new ArgumentOutOfRangeException(nameof(rarityType), rarityType, null);
        }
    }
}
