using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.ViewModels.Randomizer;

namespace RpgFilesGeneratorTools.Services;

internal sealed class ItemRandomizerProvider : IItemRandomizerProvider
{
    private readonly IItemProvider _itemProvider;
    private readonly IAffixProvider _affixProvider;
    private readonly ILogger<ItemRandomizerProvider> _logger;
    private readonly Random _random = new();

    public ItemRandomizerProvider(IItemProvider itemProvider, IAffixProvider affixProvider, ILogger<ItemRandomizerProvider> logger)
    {
        _itemProvider = itemProvider;
        _affixProvider = affixProvider;
        _logger = logger;
    }

    public async IAsyncEnumerable<RandomizedItem> GenerateRandomizedItemsAsync(
        RandomizerSettings settings,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var items = await _itemProvider.GetItemsAsync(cancellationToken).ConfigureAwait(false);
        var affixes = await _affixProvider.GetAffixesAsync(cancellationToken).ConfigureAwait(false);

        var totalWeights = settings.ItemTypeWeights.Sum(x => x.Weight) + 1;
        var cumulativeWeights = new List<ItemTypeCumulativeWeight>(settings.ItemTypeWeights.Count);
        var cumulativeWeight = 0;

        foreach (var itemTypeWeight in settings.ItemTypeWeights)
        {
            cumulativeWeight += itemTypeWeight.Weight;
            cumulativeWeights.Add(new ItemTypeCumulativeWeight(itemTypeWeight.ItemType, cumulativeWeight));
        }

        for (var i = 0; i < settings.NumberOfItemsToGenerate; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var randomType = _random.Next(1, totalWeights);

            var itemType = cumulativeWeights.First(x => x.CumulativeWeight >= randomType).ItemType;

            if (!TryGenerateRandomizedItemFromItemType(itemType, items, affixes, settings, out var item))
            {
                continue;
            }

            yield return item;
        }
    }

    private bool TryGenerateRandomizedItemFromItemType(
        ItemType itemType,
        IEnumerable<Item> items,
        IEnumerable<Affix> affixes,
        RandomizerSettings settings,
        [NotNullWhen(true)] out RandomizedItem? result)
    {
        try
        {
            var possibleItems = items.Where(x => x.Type == itemType).ToList();

            var item = possibleItems.ElementAt(_random.Next(possibleItems.Count));

            var matchingAffixes = affixes.Where(x => x.Rules.Any(r => r.ItemTypes.Contains(itemType))).ToList();

            var affix = matchingAffixes.ElementAt(_random.Next(matchingAffixes.Count));

            var affixRule = affix.Rules[_random.Next(affix.Rules.Count)];

            var mod = affixRule.Modifier1Min;

            if (int.TryParse(affixRule.Modifier1Min, out var modMin) &&
                int.TryParse(affixRule.Modifier1Max, out var modMax))
            {
                mod = $"{_random.Next(modMin, modMax + 1)}";
            }

            var rarity = GenerateRarity(settings);

            result = new RandomizedItem(item.Name, itemType, $"{affix.Name}: {mod}", rarity);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate a random drop: {Message}", exception.Message);
            result = default;
            return false;
        }
    }

    private bool TryGenerateRandomizedItemFromAffix(
        IEnumerable<Item> items,
        IReadOnlyList<Affix> affixes,
        RandomizerSettings settings,
        [NotNullWhen(true)] out RandomizedItem? result)
    {
        var index = _random.Next(1, 40);
        var affix = affixes[index];
        var tier = _random.Next(1, 6);
        var affixInfo = affix.Rules.FirstOrDefault(x => x.Tier == tier);
        var items2 = items.Where(x => affixInfo?.ItemTypes.Contains(x.Type) ?? false).ToList();
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

    private sealed record ItemTypeCumulativeWeight(ItemType ItemType, int CumulativeWeight);
}
