using LlamaRpg.Models.Affixes;
using LlamaRpg.Models.Items;
using LlamaRpg.Models.Items.PrimaryTypes;
using LlamaRpg.Models.Randomizer;
using LlamaRpg.Services.Validators;
using Microsoft.Extensions.Logging;

namespace LlamaRpg.Services.Randomization;

internal sealed class RandomizedAffixProvider : IRandomizedAffixProvider
{
    private const string Percentage = "%";

    private readonly IRandomizerAffixValidator _affixValidator;
    private readonly INumberOfAffixesGenerator _numberOfAffixesGenerator;
    private readonly ILogger<RandomizedAffixProvider> _logger;

    private readonly Random _random = new();

    public RandomizedAffixProvider(
        IRandomizerAffixValidator affixValidator,
        INumberOfAffixesGenerator numberOfAffixesGenerator,
        ILogger<RandomizedAffixProvider> logger)
    {
        _affixValidator = affixValidator;
        _numberOfAffixesGenerator = numberOfAffixesGenerator;
        _logger = logger;
    }

    public (IReadOnlyList<string> BaseAffixes, IReadOnlyList<string> Affixes) GenerateAffixes(
        ItemBase item,
        int itemPowerLevel,
        ItemRarityType rarity,
        IEnumerable<Affix> affixes,
        int monsterLevel,
        NumberOfAffixesSettings numberOfAffixesSettings)
    {
        var matchingAffixes = affixes
            .Where(x => x.Rules.Any(r => _affixValidator.ValidateRule(r, item.Type, item.Subtype, monsterLevel, itemPowerLevel)))
            .ToList();

        if (matchingAffixes.Count == 0)
        {
            _logger.LogError("Failed to generate a random drop: no matching affixes found for item type {ItemType}.", item.Type);
            return (Array.Empty<string>(), Array.Empty<string>());
        }

        var primaryElement = (PrimaryElement)_random.Next(0, 4);
        var secondaryElement = (SecondaryElement)_random.Next(0, 8);

        var baseAffixes = GenerateBaseAffixes(
            item,
            rarity,
            primaryElement,
            secondaryElement,
            itemPowerLevel,
            monsterLevel,
            matchingAffixes,
            out var affixGroupToExclude);

        if (rarity == ItemRarityType.Normal)
        {
            return (baseAffixes.AsReadOnly(), Array.Empty<string>());
        }

        var (numberOfPrefixes, numberOfSuffixes) = rarity switch
        {
            ItemRarityType.Magic => _numberOfAffixesGenerator.Generate(
                numberOfAffixesSettings.PrefixesForMagicItems,
                numberOfAffixesSettings.SuffixesForMagicItems,
                numberOfAffixesSettings.MinimumNumberOfAffixesForMagicItems),
            ItemRarityType.Rare => _numberOfAffixesGenerator.Generate(
                numberOfAffixesSettings.PrefixesForRareItems,
                numberOfAffixesSettings.SuffixesForRareItems,
                numberOfAffixesSettings.MinimumNumberOfAffixesForRareItems),
            ItemRarityType.Elite => _numberOfAffixesGenerator.GenerateForEliteItems(numberOfAffixesSettings.AffixesForEliteItems),
            _ => (0, 0)
        };

        var generatedPrefixes = InternalGenerateAffixes(
            numberOfPrefixes,
            matchingAffixes.Where(x => x.Attribute == AffixAttributeType.Prefix).ToList(),
            isBaseAffix: false,
            item,
            rarity,
            primaryElementOfItem: item.Type is ItemType.Weapon or ItemType.Offhand ? primaryElement : default,
            secondaryElementOfItem: item.Type is ItemType.Weapon or ItemType.Offhand ? secondaryElement : default,
            itemPowerLevel,
            monsterLevel,
            affixGroupToExclude,
            out _);

        var generatedSuffixes = InternalGenerateAffixes(
            numberOfSuffixes,
            matchingAffixes.Where(x => x.Attribute == AffixAttributeType.Suffix).ToList(),
            isBaseAffix: false,
            item,
            rarity,
            primaryElementOfItem: item.Type is ItemType.Weapon or ItemType.Offhand ? primaryElement : default,
            secondaryElementOfItem: item.Type is ItemType.Weapon or ItemType.Offhand ? secondaryElement : default,
            itemPowerLevel,
            monsterLevel,
            affixGroupToExclude,
            out _);

        return (baseAffixes.AsReadOnly(), generatedPrefixes.Concat(generatedSuffixes).ToList().AsReadOnly());
    }

    private IReadOnlyList<string> InternalGenerateAffixes(
        int count,
        IReadOnlyList<Affix> matchingAffixes,
        bool isBaseAffix,
        ItemBase item,
        ItemRarityType itemRarity,
        PrimaryElement? primaryElementOfItem,
        SecondaryElement? secondaryElementOfItem,
        int itemPowerLevel,
        int itemLevelRequired,
        int? affixGroupToExclude,
        out int? affixGroup)
    {
        const int maxNumberOfAttempts = 100;
        affixGroup = null;

        var generatedAffixesWithGroup = new List<(int Group, string AffixText)>();
        var generatedAffixNames = new HashSet<int>();

        if (affixGroupToExclude is not null)
        {
            generatedAffixNames.Add(affixGroupToExclude.Value);
        }

        var overallNumberOfAttempts = 0;

        for (var i = 0; i < count; i++)
        {
            Affix? affix = null;
            AffixRule? affix1Rule = null;
            var invalidAffix = true;
            var numberOfAttempts = 0;

            while (invalidAffix)
            {
                numberOfAttempts++;
                overallNumberOfAttempts++;

                affix = matchingAffixes[_random.Next(matchingAffixes.Count)];

                var rules = affix.Rules.Where(r => !generatedAffixNames.Contains(r.Group)
                                                   && _affixValidator.ValidateRule(r, item.Type, item.Subtype, itemLevelRequired, itemPowerLevel)).ToList();

                if (rules.Count == 0)
                {
                    continue;
                }

                affix1Rule = affix.Rules[_random.Next(affix.Rules.Count)];

                affixGroup = count == 1 ? affix1Rule.Group : null;

                invalidAffix = _affixValidator.ValidateItemElements(affix, primaryElementOfItem, secondaryElementOfItem) == false
                               || (isBaseAffix && affix1Rule.PowerLevelRequired != itemPowerLevel)
                               || _affixValidator.ValidateRarity(affix1Rule, itemRarity) == false
                               || _affixValidator.ValidateRule(affix1Rule, item.Type, item.Subtype, itemLevelRequired, itemPowerLevel) == false;

                if (numberOfAttempts > maxNumberOfAttempts)
                {
                    throw new InvalidOperationException($"Impossible to find affix for {itemRarity} item {item.Name} (plvl={itemLevelRequired})");
                }
            }

            if (affix is null || affix1Rule is null)
            {
                throw new InvalidOperationException($"Impossible to find affix for item {item.Name} (plvl={itemLevelRequired})");
            }

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
                    max = (itemPowerLevel * 3) + weapon.MaxBlock;

                    EnsureMaxValue(affix1Rule.Modifier1Text, affix1Rule.Modifier1MinText, min, ref max);

                    mod = affix1Rule.Variance == AffixVariance.MinAndMaxInterval
                        ? $"{min} to {max}"
                        : $"+{_random.Next(min, max + 1)}";
                    break;

                case AffixModifierType.PowerLevelPlusMinimumBlock when item is Offhand offhand:
                    min = itemPowerLevel + offhand.MinBlock;
                    max = (itemPowerLevel * 3) + offhand.MaxBlock;

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

            generatedAffixesWithGroup.Add((affix1Rule.Group, $"{affix.Name}: {mod}{(affix.HasPercentageModifier ? Percentage : string.Empty)}"));
            generatedAffixNames.Add(affix1Rule.Group);
        }

        _logger.LogDebug("Generated affixes after {Attempts} attempts", overallNumberOfAttempts);

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
                modifierCode,
                modifierText);
        }
    }

    private List<string> GenerateBaseAffixes(
        ItemBase item,
        ItemRarityType rarity,
        PrimaryElement primaryElement,
        SecondaryElement secondaryElement,
        int itemPowerLevel,
        int itemLevelRequired,
        IReadOnlyCollection<Affix> matchingAffixes,
        out int? affixGroupToExclude)
    {
        var baseAffixes = new List<string>(); // TODO: Yield return

        affixGroupToExclude = default;

        IReadOnlyList<Affix> mandatoryAffixes = item.Type switch
        {
            ItemType.Weapon =>
                matchingAffixes
                    .Where(x => x.Type == AffixType.ElementalDamage)
                    .Where(x => x.Name.Contains(secondaryElement.ToString(), StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Offhand =>
                matchingAffixes
                    .Where(x => x.Type == AffixType.ElementalDefense)
                    .Where(x => x.Name.Contains(primaryElement.ToString(), StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .AsReadOnly(),

            ItemType.Armor =>
                matchingAffixes
                    .Where(x => x.Type == AffixType.Defense)
                    .ToList()
                    .AsReadOnly(),
            ItemType.Jewelry or ItemType.MeleeWeapon or ItemType.MagicWeapon or ItemType.RangeWeapon or _ => Enumerable.Empty<Affix>().ToList()
        };

        var generatedBaseAffixes = mandatoryAffixes.Count > 0
            ? InternalGenerateAffixes(
                count: 1,
                mandatoryAffixes,
                isBaseAffix: true,
                item,
                rarity,
                primaryElementOfItem: default,
                secondaryElementOfItem: default,
                itemPowerLevel,
                itemLevelRequired,
                default,
                out affixGroupToExclude)
            : Array.Empty<string>();

        if (generatedBaseAffixes.Count > 0)
        {
            baseAffixes.Add(generatedBaseAffixes[0]);
        }

        if (item.Subtype is not (ItemSubtype.Wand or ItemSubtype.Staff))
        {
            return baseAffixes;
        }

        mandatoryAffixes = matchingAffixes
            .Where(x => x.Type == AffixType.ElementalDefense && x.PrimaryElement == primaryElement)
            .ToList()
            .AsReadOnly();

        var additionalBaseAffixes = InternalGenerateAffixes(
            count: 1,
            mandatoryAffixes,
            isBaseAffix: false,
            item,
            rarity,
            primaryElementOfItem: default,
            secondaryElementOfItem: default,
            itemPowerLevel,
            itemLevelRequired,
            default,
            out _);

        if (additionalBaseAffixes.Count > 0)
        {
            baseAffixes.Add(additionalBaseAffixes[0]);
        }

        return baseAffixes;
    }
}
