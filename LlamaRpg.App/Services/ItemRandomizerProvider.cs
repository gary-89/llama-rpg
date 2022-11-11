using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LlamaRpg.App.ViewModels.Randomizer;
using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;
using LlamaRpg.Services.Randomization;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.App.Services;

internal sealed class ItemRandomizerProvider : IItemRandomizerProvider
{
    private const string Damage = "damage";
    private const string Defense = "defense";

    private readonly IItemProvider _itemProvider;
    private readonly IAffixProvider _affixProvider;
    private readonly IRandomizerAffixValidator _validator;
    private readonly ILogger<ItemRandomizerProvider> _logger;
    private readonly Random _random = new();
    private readonly IReadOnlyList<string> _primaryElements;
    private readonly IReadOnlyList<string> _secondaryElements;

    public ItemRandomizerProvider(
        IItemProvider itemProvider,
        IAffixProvider affixProvider,
        IRandomizerAffixValidator validator,
        ILogger<ItemRandomizerProvider> logger)
    {
        _itemProvider = itemProvider;
        _affixProvider = affixProvider;
        _validator = validator;
        _logger = logger;

        _primaryElements = Enum.GetNames(typeof(PrimaryElement));
        _secondaryElements = Enum.GetNames(typeof(SecondaryElement));
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

            var rarity = GenerateRarity(settings, item.Type == ItemType.Jewelry);

            var powerLevel = GeneratePowerLevel(settings.MonsterLevel);

            var (baseAffixes, generatedAffixes) = GenerateAffixes(item, powerLevel, rarity, affixes, settings);

            result = new RandomizedItem(counter, item.Name, itemType, item.Subtype, powerLevel, baseAffixes, generatedAffixes, rarity);

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

    private ItemRarityType GenerateRarity(RandomizerSettings settings, bool isJewelry)
    {
        var rarity = _random.Next(0, settings.EliteItemDropRate * settings.RareItemDropRate * settings.MagicItemDropRate);

        return rarity % settings.EliteItemDropRate == 0
            ? ItemRarityType.Elite
            : rarity % settings.RareItemDropRate == 0
                ? ItemRarityType.Rare
                : rarity % settings.MagicItemDropRate == 0
                    ? ItemRarityType.Magic
                    : isJewelry
                        ? ItemRarityType.Magic
                        : ItemRarityType.Normal;
    }

    private (IReadOnlyList<string> BaseAffixes, IReadOnlyList<string> Affixes) GenerateAffixes(
        ItemBase item,
        int itemPowerLevel,
        ItemRarityType rarity,
        IEnumerable<Affix> affixes,
        RandomizerSettings settings)
    {
        var itemType = item.Type;
        var itemSubtype = item.Subtype;

        var matchingAffixes = affixes
            .Where(x => x.Rules
                .Any(r => r.ItemLevelRequired < settings.MonsterLevel
                          && r.PowerLevelRequired <= itemPowerLevel
                          && (r.ItemTypes.Contains(itemType)
                              || r.ItemSubtypes.Contains(itemSubtype)
                              || (r.ItemTypes.Contains(ItemType.MeleeWeapon) && (itemSubtype is ItemSubtype.Axe or ItemSubtype.Sword or ItemSubtype.Mace))
                              || (r.ItemTypes.Contains(ItemType.RangeWeapon) && (itemSubtype is ItemSubtype.Bow or ItemSubtype.Crossbow))
                              || (r.ItemTypes.Contains(ItemType.MagicWeapon) && (itemSubtype is ItemSubtype.Wand or ItemSubtype.Staff)))))
            .ToList();

        if (matchingAffixes.Count == 0)
        {
            _logger.LogError("Failed to generate a random drop: no matching affixes found for item type {ItemType}.", itemType);
            throw new InvalidOperationException($"Failed to generate a random drop: no matching affixes found for item type {itemType}.");
        }

        var primaryElement = (PrimaryElement)_random.Next(0, 4);
        var secondaryElement = (SecondaryElement)_random.Next(0, 8);

        var baseAffixes = GenerateBaseAffixes(item, primaryElement, secondaryElement, itemPowerLevel, matchingAffixes, out var affixGroupToExclude);

        if (rarity == ItemRarityType.Normal)
        {
            return (baseAffixes.AsReadOnly(), Array.Empty<string>());
        }

        var numberOfAffixes = rarity switch
        {
            ItemRarityType.Magic => _random.Next(settings.AffixesForMagicItems.Min, settings.AffixesForMagicItems.Max + 1),
            ItemRarityType.Rare => _random.Next(settings.AffixesForRareItems.Min, settings.AffixesForRareItems.Max + 1),
            ItemRarityType.Elite => _random.Next(settings.AffixesForEliteItems.Min, settings.AffixesForEliteItems.Max + 1),
            _ => 0
        };

        var generatedAffixes = InternalGenerateAffixes(
            item,
            itemType == ItemType.Weapon ? primaryElement : default,
            itemType == ItemType.Weapon ? secondaryElement : default,
            itemPowerLevel,
            numberOfAffixes,
            matchingAffixes.Where(x => x.Rules.Any(r => _validator.ValidateRarity(r, rarity))).ToList(),
            affixGroupToExclude, out _);

        return (baseAffixes.AsReadOnly(), generatedAffixes);
    }

    private List<string> GenerateBaseAffixes(
        ItemBase item,
        PrimaryElement primaryElement,
        SecondaryElement secondaryElement,
        int itemPowerLevel,
        IReadOnlyCollection<Affix> matchingAffixes,
        out int? affixGroupToExclude)
    {
        var baseAffixes = new List<string>();

        IReadOnlyCollection<Affix> mandatoryAffixes = item.Type switch
        {
            ItemType.Weapon =>
                matchingAffixes
                    .Where(x => x.Name.Contains(Damage, StringComparison.OrdinalIgnoreCase))
                    .Where(x => x.Name.Contains(secondaryElement.ToString(), StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Offhand =>
                matchingAffixes
                    .Where(x => x.Name.Contains(Defense, StringComparison.OrdinalIgnoreCase))
                    .Where(x => x.Name.Contains(primaryElement.ToString(), StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Armor =>
                matchingAffixes
                    .Where(x => x.Name.Equals(Defense, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Jewelry => Enumerable.Empty<Affix>().ToList(),
            ItemType.MeleeWeapon => Enumerable.Empty<Affix>().ToList(),
            ItemType.MagicWeapon => Enumerable.Empty<Affix>().ToList(),
            ItemType.RangeWeapon => Enumerable.Empty<Affix>().ToList(),
            _ => Enumerable.Empty<Affix>().ToList()
        };

        var baseAffix = InternalGenerateAffixes(
            item,
            default,
            default,
            itemPowerLevel,
            count: 1,
            mandatoryAffixes,
            default,
            out affixGroupToExclude).FirstOrDefault();

        if (baseAffix is not null)
        {
            baseAffixes.Add(baseAffix);
        }

        if (item.Subtype is not (ItemSubtype.Wand or ItemSubtype.Staff))
        {
            return baseAffixes;
        }

        mandatoryAffixes = matchingAffixes
            .Where(x => x.Name.Contains(Defense, StringComparison.OrdinalIgnoreCase))
            .Where(x => x.Name.Contains(primaryElement.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        var baseAffix2 = InternalGenerateAffixes(
            item,
            default,
            default,
            itemPowerLevel,
            count: 1,
            mandatoryAffixes,
            default,
            out _).FirstOrDefault();

        if (baseAffix2 is not null)
        {
            baseAffixes.Add(baseAffix2);
        }

        return baseAffixes;
    }

    private IReadOnlyList<string> InternalGenerateAffixes(
        ItemBase item,
        PrimaryElement? primaryElementOfWeapon,
        SecondaryElement? secondaryElementOfWeapon,
        int itemPowerLevel,
        int count,
        IReadOnlyCollection<Affix> matchingAffixes,
        int? affixGroupToExclude,
        out int? affixGroup)
    {
        List<(int Group, string AffixText)> generatedAffixesWithGroup = new();
        affixGroup = null;

        if (matchingAffixes.Count == 0)
        {
            return Array.Empty<string>();
        }

        HashSet<int> generatedAffixNames = new();

        if (affixGroupToExclude is not null)
        {
            generatedAffixNames.Add(affixGroupToExclude.Value);
        }

        for (var i = 0; i < count; i++)
        {
            Affix affix;
            AffixRule affix1Rule;
            bool invalidAffix;
            do
            {
                affix = matchingAffixes.ElementAt(_random.Next(matchingAffixes.Count));
                affix1Rule = affix.Rules[_random.Next(affix.Rules.Count)];
                affixGroup = count == 1 ? affix1Rule.Group : null;

                invalidAffix = (secondaryElementOfWeapon.HasValue && _validator.ValidateEnhanceDamageAffix(affix, secondaryElementOfWeapon.Value) == false)
                    || itemPowerLevel < affix1Rule.PowerLevelRequired;

            } while (invalidAffix || generatedAffixNames.Contains(affix1Rule.Group));

            var mod = affix1Rule.Modifier1MinText;
            int min, max;

            switch (affix1Rule.Type)
            {
                case AffixModifierType.Number:
                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{affix1Rule.Modifier1Min} to {affix1Rule.Modifier1Max}"
                        : $"+{_random.Next(affix1Rule.Modifier1Min, affix1Rule.Modifier1Max + 1)}";
                    break;

                case AffixModifierType.MinimumDamagePlus when item is Weapon weapon:
                    min = weapon.MinDamage + affix1Rule.Modifier1Min;
                    max = weapon.MaxDamage * itemPowerLevel;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.MinimumBlockPlus when item is Weapon weapon:
                    min = weapon.MinBlock + affix1Rule.Modifier1Min;
                    max = weapon.MaxBlock * itemPowerLevel;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.MinimumBlockPlus when item is Offhand offhand:
                    min = offhand.MinBlock + affix1Rule.Modifier1Min;
                    max = offhand.MaxBlock * itemPowerLevel;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.PowerLevelPlusMinimumBlock when item is Weapon weapon:
                    min = itemPowerLevel + weapon.MinBlock;
                    max = itemPowerLevel * 3 + weapon.MaxBlock;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.PowerLevelPlusMinimumBlock when item is Offhand offhand:
                    min = itemPowerLevel + offhand.MinBlock;
                    max = itemPowerLevel * 3 + offhand.MaxBlock;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.Undefined:
                default:
                    // Ignore
                    break;
            }

            generatedAffixesWithGroup.Add((affix1Rule.Group, $"{affix.Name}: {mod}"));
            generatedAffixNames.Add(affix1Rule.Group);
        }

        return generatedAffixesWithGroup.OrderBy(x => x.Group).Select(x => x.AffixText).ToList().AsReadOnly();

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
