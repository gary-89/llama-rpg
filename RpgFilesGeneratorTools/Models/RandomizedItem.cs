using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal sealed record RandomizedItem(
    string ItemName,
    ItemType ItemType,
    ItemSubtype ItemSubtype,
    IReadOnlyList<string> Affixes,
    ItemRarityType ItemRarityType);
