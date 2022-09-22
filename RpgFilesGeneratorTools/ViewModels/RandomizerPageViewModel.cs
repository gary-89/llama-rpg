using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Services;
using RpgFilesGeneratorTools.Toolkit.Async;

namespace RpgFilesGeneratorTools.ViewModels;

internal sealed class RandomizerPageViewModel : ObservableObject
{
    private readonly IAffixProvider _affixProvider;
    private readonly IItemProvider _itemProvider;
    private readonly AsyncRelayCommand _randomizeCommand;
    private readonly Random _random;

    private IReadOnlyList<Item> _items = Array.Empty<Item>();
    private IReadOnlyList<Affix> _affixes = Array.Empty<Affix>();

    public RandomizerPageViewModel(IAffixProvider affixProvider, IItemProvider itemProvider)
    {
        _affixProvider = affixProvider;
        _itemProvider = itemProvider;
        _random = new Random();
        _randomizeCommand = new AsyncRelayCommand(GenerateRandomizedItemsAsync, CanRandomizeItems);

        ResetCommand = new RelayCommand(() => GeneratedItems.Clear());

        TaskCompletion = new NotifyTaskCompletion<int>(InitializeAsync());
    }

    public NotifyTaskCompletion<int> TaskCompletion { get; }

    public ICommand RandomizeCommand => _randomizeCommand;

    public ICommand ResetCommand { get; }

    public ObservableCollection<RandomizedItem> GeneratedItems { get; } = new();

    private async Task<int> InitializeAsync()
    {
        _affixes = await _affixProvider.GetAffixesAsync(CancellationToken.None).ConfigureAwait(true);
        _items = await _itemProvider.GetItemsAsync(CancellationToken.None).ConfigureAwait(true);
        _randomizeCommand.NotifyCanExecuteChanged();
        return 0;
    }

    private bool CanRandomizeItems()
    {
        return _affixes.Count > 0 && _items.Count > 0;
    }

    private async Task GenerateRandomizedItemsAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 50; i++)
        {
            await Task.Delay(100, cancellationToken);

            if (TryGenerateRandomizedItem(out var item))
            {
                GeneratedItems.Add(item);
            }
        }
    }

    private bool TryGenerateRandomizedItem([NotNullWhen(true)] out RandomizedItem? result)
    {
        var index = _random.Next(1, 40);
        var affix = _affixes[index];
        var tier = _random.Next(1, 6);
        var affixInfo = affix.Rules.FirstOrDefault(x => x.Tier == tier);
        var items = _items.Where(x => affixInfo?.ItemType.Contains(x.Type, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        var item = items.Count > 0 ? items[_random.Next(0, items.Count - 1)] : null;

        if (affixInfo is null || item is null)
        {
            result = default;
            return false;
        }

        var mod = affixInfo.Modifier1Min;

        if (int.TryParse(affixInfo.Modifier1Min, out var modMin) &&
            int.TryParse(affixInfo.Modifier1Min, out var modMax))
        {
            mod = $"{_random.Next(modMin, modMax)}";
        }

        var rarity = _random.Next(1, 50);

        result = new RandomizedItem(
            item.Name,
            $"{affix.Name}: {mod}",
            rarity == 1 ? ItemRarityType.Unique : rarity % 5 == 0 ? ItemRarityType.Rare : ItemRarityType.Normal);

        return true;
    }
}
