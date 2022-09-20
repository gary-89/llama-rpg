using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.ViewModels;

internal class AffixesPageViewModel : ObservableObject
{
    private readonly IItemProvider _itemProvider;
    private readonly IAffixProvider _affixProvider;
    private readonly ILogger<AffixesPageViewModel> _logger;

    private string? _filterText;
    private Affix? _selectedAffix;

    public AffixesPageViewModel(IItemProvider itemProvider, IAffixProvider affixProvider, ILogger<AffixesPageViewModel> logger)
    {
        _itemProvider = itemProvider;
        _affixProvider = affixProvider;
        _logger = logger;

        TaskInitialize = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> TaskInitialize { get; }

    public ObservableCollection<Affix> AffixesSource { get; } = new();

    public string? FilterText
    {
        get => _filterText;
        set => SetProperty(ref _filterText, value);
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
            var items = await _itemProvider.GetItemsAsync(CancellationToken.None);

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
