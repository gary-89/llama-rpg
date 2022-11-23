namespace LlamaRpg.Models.Randomizer;

public sealed record NumberOfAffixesSettings(
    Range SuffixesForMagicItems,
    Range PrefixesForMagicItems,
    int MinimumNumberOfAffixesForMagicItems,
    Range SuffixesForRareItems,
    Range PrefixesForRareItems,
    int MinimumNumberOfAffixesForRareItems,
    Range AffixesForEliteItems)
{
    public static NumberOfAffixesSettings Default()
        => new(
            new Range(0, 1),
            new Range(0, 1),
            1,
            new Range(1, 2),
            new Range(1, 2),
            3,
            new Range(3, 4));
}
