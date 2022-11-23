using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Randomizer;

public sealed record ItemRandomizerSettings(
    int NumberOfItemsToGenerate,
    int MonsterLevel,
    DropRateSettings DropRateSettings,
    NumberOfAffixesSettings NumberOfAffixesSettings,
    IReadOnlyList<ItemTypeWeightDrop> ItemTypeWeights);
