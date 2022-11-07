using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Affixes;

public sealed record AffixRule(
    IReadOnlyList<ItemType> ItemTypes,
    IReadOnlyList<ItemSubtype> ItemSubtypes,
    int Tier,
    bool IsRare,
    bool IsElite,
    bool IsEliteOnly,
    int ItemLevelRequired,
    int PowerLevelRequired,
    int Group,
    int MaxLevel,
    int Frequency,
    string Modifier1Text,
    string Modifier1MinText,
    string Modifier1MaxText,
    AffixModifierType Type,
    int Modifier1Min,
    int Modifier1Max,
    AffixVariance Variance);
