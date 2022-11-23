using LlamaRpg.Models.Items;
using Range = LlamaRpg.Models.Range;

namespace LlamaRpg.Services.Randomization;

internal sealed class NumberOfAffixesGenerator : INumberOfAffixesGenerator
{
    private readonly Random _random = new();

    public (int NumberOfPrefixes, int NumberOfAffixes) Generate(Range numberOfPrefixes, Range numberOfSuffixes, int mandatoryAffixes)
    {
        var safeCounter = 0;
        var numberOfSuffixesToGenerateCounter = 0;
        var numberOfPrefixesToGenerateCounter = 0;

        var prefixesToGenerate = _random.Next(numberOfPrefixes.Min, numberOfPrefixes.Max + 1);
        var suffixesToGenerate = _random.Next(numberOfSuffixes.Min, numberOfSuffixes.Max + 1);

        while (prefixesToGenerate + suffixesToGenerate < mandatoryAffixes)
        {
            if (_random.Next(0, 100) % 2 == 0)
            {
                prefixesToGenerate++;
            }
            else
            {
                suffixesToGenerate++;
            }
        }

        while (true)
        {
            var attribute = numberOfSuffixesToGenerateCounter < suffixesToGenerate && numberOfPrefixesToGenerateCounter < prefixesToGenerate
                ? _random.Next(1, 101) % 2 == 0 ? AffixAttributeType.Suffix : AffixAttributeType.Prefix
                : numberOfPrefixesToGenerateCounter >= prefixesToGenerate
                    ? AffixAttributeType.Suffix
                    : AffixAttributeType.Prefix;

            switch (attribute)
            {
                case AffixAttributeType.Suffix:
                    numberOfSuffixesToGenerateCounter++;
                    break;

                case AffixAttributeType.Prefix:
                    numberOfPrefixesToGenerateCounter++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (numberOfSuffixesToGenerateCounter + numberOfPrefixesToGenerateCounter >= mandatoryAffixes)
            {
                if (prefixesToGenerate == numberOfPrefixesToGenerateCounter &&
                    suffixesToGenerate == numberOfSuffixesToGenerateCounter)
                {
                    break;
                }

                var maxAffixes = numberOfPrefixes.Max + numberOfSuffixes.Max;

                if (numberOfSuffixesToGenerateCounter + numberOfPrefixesToGenerateCounter >= maxAffixes)
                {
                    break;
                }

                var continueToReachMaxNumberOfAffixes = _random.Next(0, 100) > 40;

                if (continueToReachMaxNumberOfAffixes == false)
                {
                    break;
                }
            }

            safeCounter++;

            if (safeCounter > 1000)
            {
                throw new OverflowException("Risque of endless loop during the generation of prefixes and suffixes cardinality.");
            }
        }

        return (numberOfPrefixesToGenerateCounter, numberOfSuffixesToGenerateCounter);
    }

    public (int NumberOfPrefixes, int NumberOfAffixes) GenerateForEliteItems(Range numberOfAffixesForEliteItems)
    {
        var eliteAffixesToGenerate = _random.Next(numberOfAffixesForEliteItems.Min, numberOfAffixesForEliteItems.Max + 1);
        var eliteAffixesToGeneratePart1 = _random.Next(0, eliteAffixesToGenerate + 1);
        var eliteAffixesToGeneratePart2 = numberOfAffixesForEliteItems.Max - eliteAffixesToGeneratePart1;

        return _random.Next(0, 100) % 2 == 0 // 50% chance to use prefix or suffix
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
