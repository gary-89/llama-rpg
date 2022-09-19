using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.ViewModels;

internal class AffixesPageViewModel : ObservableObject
{
    private readonly IAffixProvider _affixProvider;
    private readonly ILogger<AffixesPageViewModel> _logger;

    private string? _filter;
    private Affix? _selectedAffix;

    public AffixesPageViewModel(IAffixProvider affixProvider, ILogger<AffixesPageViewModel> logger)
    {
        _affixProvider = affixProvider;
        _logger = logger;

        Initialization = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> Initialization { get; }

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
            var affixes = await _affixProvider.GetAffixesAsync(CancellationToken.None);

            AffixesSource.AddEach(affixes);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to initialize affixes view model");
        }

        return 0;
    }
}
