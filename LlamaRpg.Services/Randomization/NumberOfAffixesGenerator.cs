using LlamaRpg.Models.Items;
using Range = LlamaRpg.Models.Range;

namespace LlamaRpg.Services.Randomization;

internal sealed class NumberOfAffixesGenerator : INumberOfAffixesGenerator
{
    private readonly Random _random = new();

    public (int NumberOfPrefixes, int NumberOfAffixes) Generate(Range numberOfPrefixes, Range numberOfSuffixes, int mandatoryAffixes)
    {
        var maximumPossibleAffixes = numberOfPrefixes.Max + numberOfSuffixes.Max;
        var numberOfAffixesToGenerate = _random.Next(mandatoryAffixes, maximumPossibleAffixes + 1);
        var numberOfEqualSplitOfAffixes = numberOfAffixesToGenerate / 2;

        var prefixesToGenerate = numberOfPrefixes.Max >= numberOfEqualSplitOfAffixes ? numberOfEqualSplitOfAffixes : numberOfPrefixes.Max;
        var suffixesToGenerate = numberOfSuffixes.Max >= numberOfEqualSplitOfAffixes ? numberOfEqualSplitOfAffixes : numberOfSuffixes.Max;

        if (suffixesToGenerate + prefixesToGenerate == numberOfAffixesToGenerate)
        {
            return (prefixesToGenerate, suffixesToGenerate);
        }

        var affixType = (AffixAttributeType)_random.Next(0, 2); // 50% chance to use prefix or suffix

        switch (affixType)
        {
            case AffixAttributeType.Prefix:
                prefixesToGenerate = Math.Min(numberOfPrefixes.Max, numberOfAffixesToGenerate - suffixesToGenerate);

                if (prefixesToGenerate + suffixesToGenerate == numberOfAffixesToGenerate)
                {
                    return (prefixesToGenerate, suffixesToGenerate);
                }

                suffixesToGenerate = Math.Min(numberOfSuffixes.Max, numberOfAffixesToGenerate - prefixesToGenerate);

                break;

            case AffixAttributeType.Suffix:
                suffixesToGenerate = Math.Min(numberOfSuffixes.Max, numberOfAffixesToGenerate - prefixesToGenerate);

                if (prefixesToGenerate + suffixesToGenerate == numberOfAffixesToGenerate)
                {
                    return (prefixesToGenerate, suffixesToGenerate);
                }

                prefixesToGenerate = Math.Min(numberOfPrefixes.Max, numberOfAffixesToGenerate - suffixesToGenerate);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(affixType), "Invalid affix type");
        }

        return (prefixesToGenerate, suffixesToGenerate);
    }

    public (int NumberOfPrefixes, int NumberOfAffixes) GenerateForEliteItems(Range numberOfAffixesForEliteItems)
    {
        var eliteAffixesToGenerate = _random.Next(numberOfAffixesForEliteItems.Min, numberOfAffixesForEliteItems.Max + 1);
        var eliteAffixesToGeneratePart1 = _random.Next(0, eliteAffixesToGenerate + 1);
        var eliteAffixesToGeneratePart2 = numberOfAffixesForEliteItems.Max - eliteAffixesToGeneratePart1;

        return _random.Next(0, 2) == 0 // 50% chance to use prefix or suffix
            ? Generate(
                new Range(0, eliteAffixesToGeneratePart2),
                new Range(0, eliteAffixesToGeneratePart1),
                numberOfAffixesForEliteItems.Min)
            : Generate(
                new Range(0, eliteAffixesToGeneratePart1),
                new Range(0, eliteAffixesToGeneratePart2),
                numberOfAffixesForEliteItems.Min);
    }
}
