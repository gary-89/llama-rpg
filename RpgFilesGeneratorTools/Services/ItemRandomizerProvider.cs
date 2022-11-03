using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using RpgFilesGeneratorTools.Models;
using RpgFilesGeneratorTools.Models.ItemTypes;
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

        var counter = 1;

        for (var i = 0; i < settings.NumberOfItemsToGenerate; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var randomType = _random.Next(1, totalWeights);

            var itemType = cumulativeWeights.First(x => x.CumulativeWeight >= randomType).ItemType;

            if (!TryGenerateRandomizedItemFromItemType(ref counter, itemType, items, affixes, settings, out var item))
            {
                continue;
            }

            yield return item;
        }
    }

    private bool TryGenerateRandomizedItemFromItemType(
        ref int counter,
        ItemType itemType,
        IEnumerable<ItemBase> items,
        IEnumerable<Affix> affixes,
        RandomizerSettings settings,
        [NotNullWhen(true)] out RandomizedItem? result)
    {
        try
        {
            var possibleItems = items.Where(x => x.Type == itemType).ToList();

            var item = possibleItems.ElementAt(_random.Next(possibleItems.Count));

            var rarity = GenerateRarity(settings);

            var powerLevel = GeneratePowerLevel(settings.MonsterLevel);

            var generatedAffixes = GenerateAffixes(item, powerLevel, rarity, affixes, settings);

            result = new RandomizedItem(counter, item.Name, itemType, item.Subtype, powerLevel, generatedAffixes.AffixBase, generatedAffixes.Affixes, rarity);

            counter++;

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate a random drop: {Message}", exception.Message);
            result = default;
            return false;
        }
    }

    private int GeneratePowerLevel(int monsterLevel)
    {
        const int DefaultChanceOfDropMaxPowerLevelItem = 30;

        while (true)
        {
            if (monsterLevel < 25)
            {
                return 1;
            }

            if (_random.Next(1, 100) <= DefaultChanceOfDropMaxPowerLevelItem)
            {
                return monsterLevel switch
                {
                    < 50 => 2,
                    _ => 3
                };
            }

            monsterLevel -= 25;
        }
    }

    private ItemRarityType GenerateRarity(RandomizerSettings settings)
    {
        var rarity = _random.Next(0, settings.EliteItemDropRate * settings.RareItemDropRate * settings.MagicItemDropRate);

        return rarity % settings.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % settings.RareItemDropRate == 0
                ? ItemRarityType.Rare
                : rarity % settings.MagicItemDropRate == 0
                    ? ItemRarityType.Magic
                    : ItemRarityType.Normal;
    }

    private (string? AffixBase, IReadOnlyList<string> Affixes) GenerateAffixes(
        ItemBase item,
        int itemPowerLevel,
        ItemRarityType rarity,
        IEnumerable<Affix> affixes,
        RandomizerSettings settings)
    {
        var itemType = item.Type;
        var itemSubtype = item.Subtype;

        var matchingAffixes = affixes.Where(x => x.Rules.Any(r => r.ItemTypes.Contains(itemType) || r.ItemSubtypes.Contains(itemSubtype))).ToList();

        if (matchingAffixes.Count == 0)
        {
            _logger.LogError("Failed to generate a random drop: no matching affixes found for item type {ItemType}.", itemType);
            throw new InvalidOperationException($"Failed to generate a random drop: no matching affixes found for item type {itemType}.");
        }

        IReadOnlyCollection<Affix> mandatoryAffixes;

        var elementalSecondaryAffixes = new[] { "Burn", "Heat", "Water", "Ice", "Electric", "Spark", "Acid", "Venom" };
        var elementalPrimaryAffixes = new[] { "Fire", "Cold", "Lighting", "Poison" };

        switch (itemType)
        {
            case ItemType.Weapon:
                mandatoryAffixes = matchingAffixes
                    .Where(x => x.Name.Contains("damage", StringComparison.OrdinalIgnoreCase))
                    .Where(x => elementalSecondaryAffixes.Any(element => x.Name.Contains(element, StringComparison.OrdinalIgnoreCase)))
                    .ToList().AsReadOnly();
                break;

            case ItemType.Offhand:
                mandatoryAffixes = matchingAffixes
                    .Where(x => x.Name.Contains("defense", StringComparison.OrdinalIgnoreCase))
                    .Where(x => elementalPrimaryAffixes.Any(element => x.Name.Contains(element, StringComparison.OrdinalIgnoreCase)))
                    .ToList().AsReadOnly();
                break;

            case ItemType.Armor:
                mandatoryAffixes = matchingAffixes
                    .Where(x => x.Name.Equals("defense", StringComparison.OrdinalIgnoreCase))
                    .ToList().AsReadOnly();
                break;

            default:
                mandatoryAffixes = Enumerable.Empty<Affix>().ToList();
                break;
        }

        var mandatoryAffix = InternalGenerateAffixes(item, itemPowerLevel, count: 1, mandatoryAffixes).FirstOrDefault();

        if (rarity == ItemRarityType.Normal)
        {
            return (mandatoryAffix, Array.Empty<string>());
        }

        var numberOfAffixes = rarity switch
        {
            ItemRarityType.Magic => _random.Next(settings.AffixesForMagicItems.Min + (mandatoryAffix is null ? 0 : -1), settings.AffixesForMagicItems.Max + 1 + (mandatoryAffix is null ? 0 : -1)),
            ItemRarityType.Rare => _random.Next(settings.AffixesForRareItems.Min + (mandatoryAffix is null ? 0 : -1), settings.AffixesForRareItems.Max + 1 + (mandatoryAffix is null ? 0 : -1)),
            ItemRarityType.Elite => _random.Next(settings.AffixesForEliteItems.Min + (mandatoryAffix is null ? 0 : -1), settings.AffixesForEliteItems.Max + 1 + (mandatoryAffix is null ? 0 : -1)),
            _ => 0
        };

        return (mandatoryAffix, InternalGenerateAffixes(item, itemPowerLevel, numberOfAffixes, matchingAffixes));
    }

    private IReadOnlyList<string> InternalGenerateAffixes(ItemBase item, int itemPowerLevel, int count, IReadOnlyCollection<Affix> matchingAffixes)
    {
        List<string> result = new();

        if (matchingAffixes.Count == 0)
        {
            return result;
        }

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
            else if (item is Weapon weapon)
            {
                if (affix1Rule.Modifier1Min.Contains("mindam", StringComparison.OrdinalIgnoreCase) &&
                    affix1Rule.Modifier1Max.Contains("maxdam * plvl", StringComparison.OrdinalIgnoreCase))
                {

                    int.TryParse(
                        affix1Rule.Modifier1Min.Replace("mindam", string.Empty).Replace("+", string.Empty)
                            .Replace(" ", string.Empty), out var minDmg);

                    min = weapon.MinDamage + minDmg;
                    max = weapon.MaxDamage * itemPowerLevel + 1;

                    mod = $"{_random.Next(min, min > max ? min + 1 : max + 1)}";
                }
            }
            else if (item is Offhand offhand)
            {
                if (affix1Rule.Modifier1Min.Contains("minblock", StringComparison.OrdinalIgnoreCase) &&
                    affix1Rule.Modifier1Max.Contains("maxblock * plvl", StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(
                        affix1Rule.Modifier1Min.Replace("minblock", string.Empty).Replace("+", string.Empty)
                            .Replace(" ", string.Empty), out var minBlock);

                    min = offhand.MinBlock + minBlock;
                    max = offhand.MaxBlock * itemPowerLevel + 1;

                    mod = $"{_random.Next(min, min > max ? min + 1 : max + 1)}";
                }
                else if (affix1Rule.Modifier1Min.Contains("plvl + minblock", StringComparison.OrdinalIgnoreCase) &&
                         affix1Rule.Modifier1Max.Contains("plvl * 3 + maxblock", StringComparison.OrdinalIgnoreCase))
                {
                    min = itemPowerLevel + offhand.MinBlock;
                    max = itemPowerLevel * 3 + offhand.MaxBlock;

                    mod = $"{_random.Next(min, min > max ? min + 1 : max + 1)}";
                }
            }

            result.Add($"{affix.Name}: +{mod}");
            generatedAffixes.Add(affix.Name);
        }

        return result;
    }

    private sealed record ItemTypeCumulativeWeight(ItemType ItemType, int CumulativeWeight);
}
