using LlamaRpg.Models.Affixes;

namespace LlamaRpg.Models.Items;

public sealed record ItemTypeAffixes(string ItemType, List<AffixRule> Affixes);
