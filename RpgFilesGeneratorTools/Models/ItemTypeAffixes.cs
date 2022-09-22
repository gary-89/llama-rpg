using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal sealed record ItemTypeAffixes(string ItemType, List<AffixRule> Affixes);
