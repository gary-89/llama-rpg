using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Toolkit.Extensions;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class RandomizerStats : ObservableObject
{
    private readonly Dictionary<ItemType, int> _generatedItemsCountPerItemType = new();

    private double _eliteGeneratedItemsCount;
    private double _rareGeneratedItemsCount;
    private double _rareGeneratedItemPercentage;
    private double _eliteGeneratedItemPercentage;

    public double TotalGeneratedItemsCount { get; set; }

    public double RareGeneratedItemsCount
    {
        get => _rareGeneratedItemsCount;
        set
        {
            if (SetProperty(ref _rareGeneratedItemsCount, value))
            {
                RareGeneratedItemPercentage = value / TotalGeneratedItemsCount;
            }
        }
    }

    public double EliteGeneratedItemsCount
    {
        get => _eliteGeneratedItemsCount;
        set
        {
            if (SetProperty(ref _eliteGeneratedItemsCount, value))
            {
                EliteGeneratedItemPercentage = value / TotalGeneratedItemsCount;
            }
        }
    }

    public double RareGeneratedItemPercentage
    {
        get => _rareGeneratedItemPercentage;
        set => SetProperty(ref _rareGeneratedItemPercentage, value);
    }

    public double EliteGeneratedItemPercentage
    {
        get => _eliteGeneratedItemPercentage;
        set => SetProperty(ref _eliteGeneratedItemPercentage, value);
    }

    public IReadOnlyList<ItemCountPerType> ItemCountPerTypes => _generatedItemsCountPerItemType.Select(x => new ItemCountPerType(x.Key, x.Value)).ToList();

    public ObservableCollection<ItemCountPerPowerLevel> ItemCountPerPowerLevels { get; } = new();

    public void UpdateOnAddingItem(RandomizedItem item)
    {
        if (!_generatedItemsCountPerItemType.ContainsKey(item.ItemType))
        {
            _generatedItemsCountPerItemType.Add(item.ItemType, 0);
        }

        _generatedItemsCountPerItemType[item.ItemType]++;

        TotalGeneratedItemsCount++;

        switch (item.ItemRarityType)
        {
            case ItemRarityType.Magic:
                RareGeneratedItemsCount++;
                break;

            case ItemRarityType.Elite:
                EliteGeneratedItemsCount++;
                break;

            case ItemRarityType.Normal:
            default:
                /* ignore */
                break;
        }
    }

    public void RefreshItemCountPerTypes()
    {
        OnPropertyChanged(nameof(ItemCountPerTypes));
    }

    public void RefreshItemCountPerPowerLevels(IEnumerable<ItemCountPerPowerLevel> itemCountPerPowerLevels)
    {
        ItemCountPerPowerLevels.Clear();
        ItemCountPerPowerLevels.AddEach(itemCountPerPowerLevels.OrderBy(x => x.PowerLevel));
    }

    public void Clear()
    {
        TotalGeneratedItemsCount = 0;
        RareGeneratedItemsCount = 0;
        EliteGeneratedItemsCount = 0;
        _generatedItemsCountPerItemType.Clear();
        ItemCountPerPowerLevels.Clear();
        RefreshItemCountPerTypes();
    }
}
