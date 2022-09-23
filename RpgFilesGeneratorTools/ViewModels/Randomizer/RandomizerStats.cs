using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class RandomizerStats : ObservableObject
{
    private double _uniqueGeneratedItemsCount;
    private double _rareGeneratedItemsCount;
    private double _rareGeneratedItemPercentage;
    private double _uniqueGeneratedItemPercentage;

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

    public double UniqueGeneratedItemsCount
    {
        get => _uniqueGeneratedItemsCount;
        set
        {
            if (SetProperty(ref _uniqueGeneratedItemsCount, value))
            {
                UniqueGeneratedItemPercentage = value / TotalGeneratedItemsCount;
            }
        }
    }

    public double RareGeneratedItemPercentage
    {
        get => _rareGeneratedItemPercentage;
        set => SetProperty(ref _rareGeneratedItemPercentage, value);
    }

    public double UniqueGeneratedItemPercentage
    {
        get => _uniqueGeneratedItemPercentage;
        set => SetProperty(ref _uniqueGeneratedItemPercentage, value);
    }
}
