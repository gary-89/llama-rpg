using CommunityToolkit.Mvvm.ComponentModel;

namespace RpgFilesGeneratorTools.ViewModels.Randomizer;

internal sealed class RandomizerStats : ObservableObject
{
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
}
