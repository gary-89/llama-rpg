using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemRandomizerProvider : IItemRandomizerProvider
{
    private readonly IItemProvider _itemProvider;
    private readonly IAffixProvider _affixProvider;
    private readonly Random _random = new();

    public ItemRandomizerProvider(IItemProvider itemProvider, IAffixProvider affixProvider)
    {
        _itemProvider = itemProvider;
        _affixProvider = affixProvider;
    }

    public async IAsyncEnumerable<RandomizedItem> GenerateRandomizedItemsAsync(
        RandomizerSettings settings,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = await _itemProvider.GetItemsAsync(cancellationToken).ConfigureAwait(false);
        var affixes = await _affixProvider.GetAffixesAsync(cancellationToken).ConfigureAwait(false);

        for (var i = 0; i < settings.NumberOfItemsToGenerate; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryGenerateRandomizedItem(items, affixes, settings, out var item))
            {
                continue;
            }

            yield return item;
        }
    }

    private bool TryGenerateRandomizedItem(
        IEnumerable<Item> items,
        IReadOnlyList<Affix> affixes,
        RandomizerSettings settings,
        [NotNullWhen(true)] out RandomizedItem? result)
    {
        var index = _random.Next(1, 40);
        var affix = affixes[index];
        var tier = _random.Next(1, 6);
        var affixInfo = affix.Rules.FirstOrDefault(x => x.Tier == tier);
        var items2 = items.Where(x => affixInfo?.ItemType.Contains(x.Type, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        var item = items2.Count > 0 ? items2[_random.Next(0, items2.Count - 1)] : null;

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

        var rarity = GenerateRarity(settings);

        result = new RandomizedItem(
            item.Name,
            item.Type,
            $"{affix.Name}: {mod}",
            rarity);

        return true;
    }

    private ItemRarityType GenerateRarity(RandomizerSettings settings)
    {
        var rarity = _random.Next(0, settings.EliteItemDropRate * settings.RareItemDropRate);

        return rarity % settings.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % settings.RareItemDropRate == 0
                ? ItemRarityType.Rare
                : ItemRarityType.Normal;
    }
}
