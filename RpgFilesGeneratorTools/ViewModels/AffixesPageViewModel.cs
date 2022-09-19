using CommunityToolkit.Mvvm.ComponentModel;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Toolkit.Async;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.ViewModels;

internal class AffixesPageViewModel : ObservableObject
{
    private readonly RandomNumberProvider _randomNumberProvider;

    private string? _filter;
    private Affix? _selectedAffix;

    public AffixesPageViewModel(RandomNumberProvider randomNumberProvider)
    {
        _randomNumberProvider = randomNumberProvider;
        Initialization = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> Initialization { get; }

    public int Number => _randomNumberProvider.GetRandomNumber();

    public ObservableCollection<Affix> AffixesSource { get; } = new();

    public string? Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    public Affix? SelectedAffix
    {
        get => _selectedAffix;
        set => SetProperty(ref _selectedAffix, value);
    }

    private async Task<int> InitializeAsync()
    {
        try
        {
            await Task.Delay(2000);
            AffixesSource.Add(new Affix("Fire resistance", 0, 15, 20, "Gloves"));
            await Task.Delay(2000);
            AffixesSource.Add(new Affix("Cold resistance", 10, 15, 20, "Boots"));
            await Task.Delay(2000);
            AffixesSource.Add(new Affix("Lighting resistance", 0, 20, 20, "Boots"));

            return 1;
        }
        catch (Exception exception)
        {
            var error = exception.Message;
            return 0;
        }
    }
}
