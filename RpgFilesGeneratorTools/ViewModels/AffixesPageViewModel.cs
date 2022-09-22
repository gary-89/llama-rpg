using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;
using RpgFilesGeneratorTools.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RpgFilesGeneratorTools.ViewModels;

internal class AffixesPageViewModel : ObservableObject
{
    private readonly IAffixProvider _affixProvider;
    private readonly ILogger<AffixesPageViewModel> _logger;
    private IReadOnlyList<Affix> _cachedAffixes = Array.Empty<Affix>();

    private string? _filterText;
    private Affix? _selectedAffix;

    public AffixesPageViewModel(IAffixProvider affixProvider, ILogger<AffixesPageViewModel> logger)
    {
        _affixProvider = affixProvider;
        _logger = logger;

        TaskInitialize = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> TaskInitialize { get; }

    public ObservableCollection<Affix> AffixesSource { get; } = new();

    public string? FilterText
    {
        get => _filterText;
        set
        {
            if (SetProperty(ref _filterText, value))
            {
                RefreshList();
            }
        }
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
            _cachedAffixes = await _affixProvider.GetAffixesAsync(CancellationToken.None);

            AffixesSource.AddEach(_cachedAffixes);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to initialize affixes view model");
        }

        return 0;
    }

    private void RefreshList()
    {
        AffixesSource.Clear();

        AffixesSource.AddEach(
            string.IsNullOrWhiteSpace(FilterText)
                ? _cachedAffixes
                : _cachedAffixes.Where(x => x.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase)));
    }
}
