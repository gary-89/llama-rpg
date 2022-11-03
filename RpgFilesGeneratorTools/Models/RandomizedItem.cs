using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal sealed record RandomizedItem(
    int Index,
    string ItemName,
    ItemType ItemType,
    ItemSubtype ItemSubtype,
    int PowerLevel,
    string? AffixBase,
    IReadOnlyList<string> Affixes,
    ItemRarityType ItemRarityType);
