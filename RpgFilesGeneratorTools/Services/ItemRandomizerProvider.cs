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
    private const string Damage = "damage";
    private const string Defense = "defense";

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

        var matchingAffixes = affixes.Where(x => x.Rules
            .Any(r => r.ItemLevelRequired < settings.MonsterLevel && (r.ItemTypes.Contains(itemType) || r.ItemSubtypes.Contains(itemSubtype))))
            .ToList();

        if (matchingAffixes.Count == 0)
        {
            _logger.LogError("Failed to generate a random drop: no matching affixes found for item type {ItemType}.", itemType);
            throw new InvalidOperationException($"Failed to generate a random drop: no matching affixes found for item type {itemType}.");
        }

        var elementalSecondaryAffixes = new[] { "Burn", "Heat", "Water", "Ice", "Electric", "Spark", "Acid", "Venom" };
        var elementalPrimaryAffixes = new[] { "Fire", "Cold", "Lighting", "Poison" };

        IReadOnlyCollection<Affix> mandatoryAffixes = itemType switch
        {
            ItemType.Weapon =>
                matchingAffixes.Where(x => x.Name.Contains(Damage, StringComparison.OrdinalIgnoreCase))
                    .Where(x => elementalSecondaryAffixes.Any(element =>
                        x.Name.Contains(element, StringComparison.OrdinalIgnoreCase)))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Offhand =>
                matchingAffixes
                    .Where(x => x.Name.Contains(Defense, StringComparison.OrdinalIgnoreCase))
                    .Where(x => elementalPrimaryAffixes.Any(element =>
                        x.Name.Contains(element, StringComparison.OrdinalIgnoreCase)))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Armor =>
                matchingAffixes.Where(x => x.Name.Equals(Defense, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Jewelry => Enumerable.Empty<Affix>().ToList(),
            ItemType.MeleeWeapon => Enumerable.Empty<Affix>().ToList(),
            ItemType.MagicWeapon => Enumerable.Empty<Affix>().ToList(),
            ItemType.RangeWeapon => Enumerable.Empty<Affix>().ToList(),
            _ => Enumerable.Empty<Affix>().ToList()
        };

        var mandatoryAffix = InternalGenerateAffixes(item, itemPowerLevel, count: 1, mandatoryAffixes, isItemAffixBase: true).FirstOrDefault();

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

    private IReadOnlyList<string> InternalGenerateAffixes(
        ItemBase item,
        int itemPowerLevel,
        int count,
        IReadOnlyCollection<Affix> matchingAffixes,
        bool isItemAffixBase = false)
    {
        List<string> generatedAffixes = new();

        if (matchingAffixes.Count == 0)
        {
            return generatedAffixes;
        }

        HashSet<string> generatedAffixNames = new();

        for (var i = 0; i < count; i++)
        {
            Affix affix;
            do
            {
                affix = matchingAffixes.ElementAt(_random.Next(matchingAffixes.Count));
            } while (generatedAffixNames.Contains(affix.Name));

            var affix1Rule = affix.Rules[_random.Next(affix.Rules.Count)];
            var mod = affix1Rule.Modifier1MinText;
            int min, max;

            switch (affix1Rule.Type)
            {
                case AffixModifierType.Number:
                    mod = isItemAffixBase
                        ? $"{affix1Rule.Modifier1Min} to {affix1Rule.Modifier1Max}"
                        : $"+{_random.Next(affix1Rule.Modifier1Min, affix1Rule.Modifier1Max + 1)}";
                    break;

                case AffixModifierType.MinimumDamagePlus when item is Weapon weapon:
                    min = weapon.MinDamage + affix1Rule.Modifier1Min;
                    max = weapon.MaxDamage * itemPowerLevel + 1;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = isItemAffixBase
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.MinimumBlockPlus when item is Offhand offhand:
                    min = offhand.MinBlock + affix1Rule.Modifier1Min;
                    max = offhand.MaxBlock * itemPowerLevel + 1;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = isItemAffixBase
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.PowerLevelPlusMinimumBlock when item is Offhand offhand:
                    min = itemPowerLevel + offhand.MinBlock;
                    max = itemPowerLevel * 3 + offhand.MaxBlock;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = isItemAffixBase
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.Undefined:
                default:
                    // Ignore
                    break;
            }

            generatedAffixes.Add($"{affix.Name}: {mod}");
            generatedAffixNames.Add(affix.Name);
        }

        return generatedAffixes;

        void EnsureMaxValue(string modifierCode, string modifierText, int min, ref int max)
        {
            if (max >= min)
            {
                return;
            }

            max = min;

            _logger.LogDebug(
                "Generating affix {ModifierCode}: max resulted to be less then min for {ModifierText}.",
                modifierCode, modifierText);
        }
    }

    private sealed record ItemTypeCumulativeWeight(ItemType ItemType, int CumulativeWeight);
}
