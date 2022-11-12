using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Randomizer;

public sealed record RandomizerSettings(
    int NumberOfItemsToGenerate,
    int MonsterLevel,
    ItemDropRates ItemDropRates,
    ItemNumberOfAffixes ItemNumberOfAffixes,
    IReadOnlyList<ItemTypeWeightDrop> ItemTypeWeights);
