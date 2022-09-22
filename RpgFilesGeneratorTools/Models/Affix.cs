using System.Collections.Generic;
using System.Linq;

namespace RpgFilesGeneratorTools.Models;

internal sealed class Affix
{
    private string? _types;

    public Affix(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<AffixRule> Rules { get; } = new();

    public string GetItemTypes()
    {
        return _types ??= string.Join(", ", Rules.Select(x => x.ItemType).Distinct());
    }

    public IEnumerable<ItemTypeAffixes> GetPerItemTypeAffixes() => Rules.GroupBy(x => x.ItemType).Select(x => new ItemTypeAffixes(x.Key, x.ToList()));
}
