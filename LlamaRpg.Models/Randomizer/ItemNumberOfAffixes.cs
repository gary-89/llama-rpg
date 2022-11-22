namespace LlamaRpg.Models.Randomizer;

public sealed record ItemNumberOfAffixes(
    Range SuffixesForMagicItems,
    Range PrefixesForMagicItems,
    Range SuffixesForRareItems,
    Range PrefixesForRareItems,
    Range AffixesForEliteItems)
{
    public static ItemNumberOfAffixes Default()
        => new(
            new Range(0, 1),
            new Range(0, 1),
            new Range(1, 2),
            new Range(1, 2),
            new Range(3, 4));
}
