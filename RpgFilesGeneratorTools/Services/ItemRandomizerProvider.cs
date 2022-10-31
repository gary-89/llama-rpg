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

            var rarity = GenerateRarity(settings);

            var generatedAffixes = GenerateAffixes(itemType, item.Subtype, rarity, affixes);

            result = new RandomizedItem(item.Name, itemType, item.Subtype, generatedAffixes, rarity);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate a random drop: {Message}", exception.Message);
            result = default;
            return false;
        }
    }

    private ItemRarityType GenerateRarity(RandomizerSettings settings)
    {
        var rarity = _random.Next(0, settings.EliteItemDropRate * settings.RareItemDropRate);

        return rarity % settings.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % settings.RareItemDropRate == 0
                ? ItemRarityType.Magic
                : ItemRarityType.Normal;
    }

    private IReadOnlyList<string> GenerateAffixes(ItemType itemType, ItemSubtype itemSubtype, ItemRarityType rarity, IEnumerable<Affix> affixes)
    {
        if (rarity == ItemRarityType.Normal)
        {
            return Array.Empty<string>();
        }

        var matchingAffixes = affixes.Where(x => x.Rules.Any(r => r.ItemTypes.Contains(itemType) || r.ItemSubtypes.Contains(itemSubtype))).ToList();

        if (matchingAffixes.Count == 0)
        {
            _logger.LogError("Failed to generate a random drop: no matching affixes found for item type {ItemType}.", itemType);
            throw new InvalidOperationException($"Failed to generate a random drop: no matching affixes found for item type {itemType}.");
        }

        return rarity switch
        {
            ItemRarityType.Magic => InternalGenerateAffixes(1, matchingAffixes),
            ItemRarityType.Elite => InternalGenerateAffixes(3, matchingAffixes),
            _ => throw new InvalidOperationException("Invalid item rarity: no affixed can be generated")
        };
    }

    private IReadOnlyList<string> InternalGenerateAffixes(int count, IReadOnlyCollection<Affix> matchingAffixes)
    {
        List<string> result = new();
        HashSet<string> generatedAffixes = new();

        for (var i = 0; i < count; i++)
        {
            Affix affix;
            do
            {
                affix = matchingAffixes.ElementAt(_random.Next(matchingAffixes.Count));
            } while (generatedAffixes.Contains(affix.Name));

            var affix1Rule = affix.Rules[_random.Next(affix.Rules.Count)];

            var mod = affix1Rule.Modifier1Min;

            if (int.TryParse(affix1Rule.Modifier1Min, out var min) &&
                int.TryParse(affix1Rule.Modifier1Max, out var max))
            {
                mod = $"{_random.Next(min, max + 1)}";
            }

            result.Add($"{affix.Name}: +{mod}");
            generatedAffixes.Add(affix.Name);
        }

        return result;
    }

    private sealed record ItemTypeCumulativeWeight(ItemType ItemType, int CumulativeWeight);
}
