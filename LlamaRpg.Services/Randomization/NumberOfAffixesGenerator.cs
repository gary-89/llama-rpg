using LlamaRpg.Models.Items;

namespace LlamaRpg.Services.Randomization;

internal sealed class NumberOfAffixesGenerator : INumberOfAffixesGenerator
{
    private readonly Random _random = new();

    public (int NumberOfPrefixes, int NumberOfAffixes) Generate(Models.Range numberOfPrefixes, Models.Range numberOfSuffixes, int mandatoryAffixes)
    {
        var safeCounter = 0;
        var numberOfSuffixesToGenerate = 0;
        var numberOfPrefixesToGenerate = 0;

        var prefixes = _random.Next(numberOfPrefixes.Min, numberOfPrefixes.Max + 1);
        var suffixes = _random.Next(numberOfSuffixes.Min, numberOfSuffixes.Max + 1);

        while (prefixes + suffixes < mandatoryAffixes)
        {
            if (_random.Next(0, 100) % 2 == 0)
            {
                prefixes++;
            }
            else
            {
                suffixes++;
            }
        }

        while (true)
        {
            var attribute = numberOfSuffixesToGenerate < suffixes && numberOfPrefixesToGenerate < prefixes
                ? _random.Next(1, 101) % 2 == 0 ? AffixAttributeType.Suffix : AffixAttributeType.Prefix
                : numberOfPrefixesToGenerate >= prefixes
                    ? AffixAttributeType.Suffix
                    : AffixAttributeType.Prefix;

            switch (attribute)
            {
                case AffixAttributeType.Suffix:
                    numberOfSuffixesToGenerate++;
                    break;

                case AffixAttributeType.Prefix:
                    numberOfPrefixesToGenerate++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (numberOfSuffixesToGenerate + numberOfPrefixesToGenerate >= mandatoryAffixes
                && prefixes >= numberOfPrefixesToGenerate
                && suffixes >= numberOfSuffixesToGenerate)
            {
                break;
            }

            safeCounter++;

            if (safeCounter > 1000)
            {
                throw new OverflowException("Risque of endless loop during the generation of prefixes and suffixes cardinality.");
            }
        }

        return (numberOfPrefixesToGenerate, numberOfSuffixesToGenerate);
    }
}
