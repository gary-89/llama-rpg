namespace LlamaRpg.Models.Randomizer;

public sealed record ItemNumberOfAffixes(Range AffixesForMagicItems, Range AffixesForRareItems, Range AffixesForEliteItems)
{
    public static ItemNumberOfAffixes Default() => new(new Range(1, 1), new Range(2, 3), new Range(3, 4));
}
