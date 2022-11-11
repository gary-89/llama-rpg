namespace LlamaRpg.Models.Items;

public sealed record RandomizedItem(
    int Index,
    string ItemName,
    ItemType ItemType,
    ItemSubtype ItemSubtype,
    int PowerLevel,
    IReadOnlyList<string> BaseAffixes,
    IReadOnlyList<string> Affixes,
    ItemRarityType ItemRarityType);
