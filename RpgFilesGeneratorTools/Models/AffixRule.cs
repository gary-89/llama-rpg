using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal sealed record AffixRule(
    IReadOnlyList<ItemType> ItemTypes,
    IReadOnlyList<ItemSubtype> ItemSubtypes,
    int Tier,
    bool IsRare,
    bool IsElite,
    int ItemLevelRequired,
    int PowerLevelRequired,
    int Group,
    int MaxLevel,
    int Frequency,
    string Modifier1,
    string Modifier1Min,
    string Modifier1Max);
